using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;
using LiveStreamStore.Lib.Models.Enums;
using LiveStreamStore.Lib.Models.SearchFilter;
using LiveStreamStore.Lib.Services.Comments;
using LiveStreamStore.Lib.Services.Categorys;
using LiveStreamStore.Lib.Services.Customers;
using LiveStreamStore.Lib.Services.Livestreams;
using LiveStreamStore.Lib.Services.Orders;
using LiveStreamStore.Lib.Services.Products;
using LiveStreamStore.Web.Filters;
using LiveStreamStore.Lib.Utilities;
using OfficeOpenXml;
using ClosedXML.Excel;
using LiveStreamStore.Lib.Services.Users;
using Microsoft.Extensions.Configuration;
using LiveStreamStore.Lib.Services.Stores;
using LiveStreamStore.Lib.Services.Logs;
using System.Reflection;

namespace LiveStream.Web.Controllers
{
    [AuthFilter]
    public class LivestreamController : BaseController
    {
        private readonly IHttpContextAccessor _HttpContextAccessor;
        private readonly ILivestreamService _LiveStreamService;
        private readonly IUserService _UserService;
        private readonly ICategoryService _CategoryService;
        private readonly IProductService _ProductService;
        private readonly IOrderService _OrderService;
        private readonly ICustomerService _CustomerService;
        private readonly ICommentService _CommentService;
        private readonly IStoreService _StoreService;
        private readonly IConfiguration _Configuration;
        private readonly ILogService _LogService;
        private static Dictionary<Comment, int> _CommentInLiveStream = new Dictionary<Comment, int>();
        private static Dictionary<int, List<Comment>> _CommentToDb = new Dictionary<int, List<Comment>>();
        private static Dictionary<int, Queue<Comment>> _Comments = new Dictionary<int, Queue<Comment>>();
        private readonly IHostingEnvironment _HostingEnvironment;
        public LivestreamController(IHttpContextAccessor httpContextAccessor, ILivestreamService liveStreamService, IProductService productService, IHostingEnvironment hostingEnvironment, ICustomerService customerService, IOrderService orderService, ICommentService commentService, ICategoryService categoryService, IUserService userService, IStoreService storeService, IConfiguration configuration, ILogService logService)
        {
            _HttpContextAccessor = httpContextAccessor;
            _LiveStreamService = liveStreamService;
            _UserService = userService;
            _ProductService = productService;
            _HostingEnvironment = hostingEnvironment;
            _CustomerService = customerService;
            _OrderService = orderService;
            _CommentService = commentService;
            _CategoryService = categoryService;
            _StoreService = storeService;
            _Configuration = configuration;
            _LogService = logService;
        }

        public IActionResult Index()
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (_User.RoleId == (int)ERoleUser.Admin)
                {
                    ViewBag.Stores = _StoreService.GetAllStore();
                }
                ViewBag.DomainPresale = _Configuration["LinkPreSale:Domain"];
                ViewBag.User = _UserService.GetUserStoreById(_User.Id);
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public PartialViewResult SearchLiveStreamByFilter(SearchLiveStreamFilter searchFilter)
        {
            try
            {
                if (_User.RoleId != (int)ERoleUser.Admin)
                {
                    searchFilter.StoreId = _User.StoreId ?? 0;
                }
                var totalRow = 0;
                var totalPage = 0;
                searchFilter.StartDate = searchFilter.StartDate.AddSeconds(-int.Parse(_Timezoneoffset));
                searchFilter.EndDate = searchFilter.EndDate.AddSeconds(-int.Parse(_Timezoneoffset));
                var livestreams = _LiveStreamService.GetListLiveStreamByUserStore(searchFilter);        
                if (livestreams.Any())
                {
                    totalRow = livestreams.FirstOrDefault().TotalRow.Value;
                    totalPage = (int)Math.Ceiling((float)totalRow / searchFilter.Top);
                }
                ViewBag.TotalRow = totalRow;
                ViewBag.TotalPage = totalPage;
                ViewBag.Page = searchFilter.Page;
                ViewBag.CurrentTimeZoneOffSet = _Timezoneoffset;
                return PartialView("_PartialListLiveStream", livestreams);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public IActionResult CreateOrUpdateLiveStream(LiveStreamStore.Lib.Data.DBContext.Models.LiveStream liveStream)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                if (liveStream.Id == 0)
                {
                    // create
                    liveStream.UserId = _User.Id;
                    error = _LiveStreamService.InsertLiveStream(liveStream);
                }
                else
                {
                    // update
                    error = _LiveStreamService.UpdateLiveStream(liveStream);
                }
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, liveStream));
            }
        }
        [HttpPost]
        public IActionResult CoppyLiveStream(int id)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var livestream = _LiveStreamService.GetLiveStreamById(id);

                error = _LiveStreamService.InsertCopyLiveStream(livestream);
                if (error.Code == Error.SUCCESS.Code)
                {
                    var hh = _ProductService.GetListProductByIdLiveStream(id, null);

                    error = _ProductService.InsertListProductCopyLiveStream
                                (_ProductService.GetListProductByIdLiveStream(id, null),
                                error.GetData<LiveStreamStore.Lib.Data.DBContext.Models.LiveStream>().Id);

                }
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id));
            }
        }

        [HttpPost]
        public IActionResult OpenPresale(int id)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                error = _LiveStreamService.SetPresale(id);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id));
            }
        }

        [HttpPost]
        public IActionResult UpdateStatusLiveStream(int id, ELiveStreamStatus eLiveStreamStatus)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                error = _LiveStreamService.UpdateStatusLiveStream(id, eLiveStreamStatus);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id, eLiveStreamStatus));
            }
        }

        // ------------------------------------------------------------------------

        public IActionResult Products(int idLivestream)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                ViewBag.LiveStream = _LiveStreamService.GetLiveStreamById(idLivestream);
                ViewBag.ListCategory = _CategoryService.GetAllCategory();
                ViewBag.ListProductInfo = _ProductService.GetListProductInfoByIdStore(ViewBag.LiveStream.User.StoreId);
                ViewBag.CurrentTimeZoneOffSet = _Timezoneoffset;
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public IActionResult GetListProductInfoByStore(int idStore)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var result = _ProductService.GetListProductInfoByIdStore(idStore);
                result.ForEach(x =>
                {
                    x.Category.ProductInfo = null;
                    if (x.Image != null)
                    {
                        x.Image.ProductInfo = null;
                    }
                    foreach (var item in x.Product)
                    {
                        item.ProductInfo = null;
                    }
                });
                return Json(result);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public PartialViewResult GetListProductByLiveStream(int idLivestream, string keywork)
        {
            try
            {
                var products = _ProductService.GetListProductByIdLiveStream(idLivestream, keywork);
                ViewBag.GetAllCategory = _CategoryService.GetAllCategory();
                var liveStream = _LiveStreamService.GetLiveStreamById(idLivestream);
                ViewBag.ListProductInfo = _ProductService.GetListProductInfoByIdStore((int)liveStream.User.StoreId);
                return PartialView("_PartialListProduct", products);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public IActionResult CreateOrUpdateProduct(Product product, ProductInfo productInfo, int idLivestream, IFormFile formFile)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                LiveStreamStore.Lib.Data.DBContext.Models.File file = new LiveStreamStore.Lib.Data.DBContext.Models.File();
                string domainName = "https://" + _HttpContextAccessor.HttpContext.Request.Host.Value;
                string WebRootPath = _HostingEnvironment.WebRootPath;
                string foldelSave = "products";

                if (product.Id == 0)
                {
                    // create
                    error = _ProductService.InsertProduct(product, productInfo, idLivestream, formFile, file, domainName, WebRootPath, foldelSave);
                }
                else
                {
                    // update
                    error = _ProductService.UpdateProduct(product, productInfo, idLivestream, formFile, file, domainName, WebRootPath, foldelSave);
                }
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, product, productInfo, idLivestream, formFile));
            }
        }

        [HttpPost]
        public IActionResult UpdateStatusProductInfo(int id, EProductStatus eProductStatus)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                error = _ProductService.UpdateStatusProductInfo(id, eProductStatus);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id, eProductStatus));
            }
        }

        [HttpPost]
        public IActionResult UpdateOneFieldProduct(Product product, ProductInfo productInfo, int idLiveStream, IFormFile formFile)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                LiveStreamStore.Lib.Data.DBContext.Models.File file = new LiveStreamStore.Lib.Data.DBContext.Models.File();
                string domainName = "https://" + _HttpContextAccessor.HttpContext.Request.Host.Value;
                string WebRootPath = _HostingEnvironment.WebRootPath;
                string foldelSave = "products";

                return Json(_ProductService.UpdateOneFieldProduct(product, productInfo, idLiveStream, formFile, file, domainName, WebRootPath, foldelSave));
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }
        [HttpPost]
        public IActionResult CreateListProduct(List<Product> product, List<ProductInfo> productInfo, int idLivestream, List<IFormFile> formFile, List<bool> existsImg)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                LiveStreamStore.Lib.Data.DBContext.Models.File file = new LiveStreamStore.Lib.Data.DBContext.Models.File();
                string domainName = "https://" + _HttpContextAccessor.HttpContext.Request.Host.Value;
                string WebRootPath = _HostingEnvironment.WebRootPath;
                string foldelSave = "products";
                error = _ProductService.InsertListProduct(product, productInfo, idLivestream, formFile, file, domainName, WebRootPath, foldelSave, existsImg);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, product, idLivestream, formFile));
            }
        }

        [HttpPost]
        public IActionResult UpdateStatusProduct(int id, EProductStatus eProductStatus)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                error = _ProductService.UpdateStatusProduct(id, eProductStatus);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id, eProductStatus));
            }
        }

        // ------------------------------------------------------------------------

        public IActionResult Comments(int id)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                var products = _ProductService.GetListProductByIdLiveStream(id, null);
                var listComment = _CommentInLiveStream?.Where(x => x.Value == id).Select(x => x.Key).ToList();
                if (listComment == null || !listComment.Any())
                {
                    listComment = _CommentService.GetCommentByLiveStreamId(id);
                }
                ViewBag.products = products;
                ViewBag.LiveStream = _LiveStreamService.GetLiveStreamById(id);
                ViewBag.ListCategory = _CategoryService.GetAllCategory();
                ViewBag.ListProductInfo = _ProductService.GetListProductInfoByIdStore(ViewBag.LiveStream.User.StoreId);
                ViewBag.CurrentTimeZoneOffSet = _Timezoneoffset;
                var result = _CustomerService.GetCustomerForComment(listComment);
                return View(result);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
        }

        [HttpPost]
        public IActionResult RefreshProduct(int id)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                var products = _ProductService.GetListProductByIdLiveStream(id, null);
                products.ForEach(x =>
                {
                    x.ProductInfo.Category = null;
                });
                return Json(products);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(err.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id));
            }
        }

        [HttpPost]
        public ActionResult GetCmtFromLiveStream(string link, int livestreamId)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                // !_Comments.ContainsKey(livestreamId): check server sập. 
                if (!_LiveStreamService.CheckIsStreamming(livestreamId) || !_Comments.ContainsKey(_User.Id))
                {
                    if (!_Comments.ContainsKey(_User.Id))
                    {
                        _Comments.Add(_User.Id, new Queue<Comment>());
                    }
                    var streamId = RegexStreamIdInLink(link);
                    var facebookToken = _UserService.GetFacebookTokenByLiveStreamId(livestreamId);
                    string apiUrl = "https://streaming-graph.facebook.com/" + streamId + "/live_comments?access_token=" + facebookToken + "&comment_rate=one_hundred_per_second&fields=from{name,id},message,created_time";
                    HttpWebRequest request = WebRequest.Create(apiUrl) as HttpWebRequest;
                    using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                    {
                        var error = _LiveStreamService.UpdateIsStreamingLiveStream(livestreamId, link, true);
                        var livestream = error.GetData<LiveStreamStore.Lib.Data.DBContext.Models.LiveStream>();
                        if (_User.Id == livestream?.UserId)
                        {
                            Task.Run(() =>
                            {
                                _CommentToDb.Add(livestreamId, new List<Comment>());
                                SaveCommentToDb(livestreamId);
                            });
                        }
                        StreamReader reader = new StreamReader(response.GetResponseStream());
                        while (_LiveStreamService.CheckIsStreamming(livestreamId))
                        {
                            var vals = reader.ReadLine();
                            if (vals != ": ping" && vals != string.Empty)
                            {
                                vals = vals.Remove(0, 6);
                                var responseJOject = JsonConvert.DeserializeObject<JObject>(vals);
                                var cmt = responseJOject["message"].ToString();
                                var cmtId = responseJOject["id"].ToString();
                                var y = responseJOject["from"];
                                var fbId = y["id"].ToString();
                                var name = y["name"].ToString();
                                var createTime = DateTime.Parse(responseJOject["created_time"].ToString());
                                var comment = new Comment
                                {
                                    CommentContent = cmt,
                                    FaceBookId = fbId,
                                    FaceBookName = name,
                                    CreatedDateUtc = createTime,
                                    CommentFaceBookId = cmtId,
                                    LiveStreamId = livestreamId
                                };
                                if (_User.Id == livestream?.UserId)
                                {
                                    _CommentInLiveStream.Add(comment, livestreamId);
                                    _CommentToDb[livestreamId].Add(comment);
                                }
                                _Comments[_User.Id].Enqueue(comment);
                            }
                        }
                        return Json(err.SetData("Livestream đang diễn ra"));
                    }
                }
                return Json(err.SetData("Livestream đang diễn ra"));
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(err.Failed("Link sai hoặc lỗi hệ thống!"));
            }
        }

        public void SaveCommentToDb(int liveStreamId)
        {
            while (_LiveStreamService.CheckIsStreamming(liveStreamId))
            {
                if (_CommentToDb.ContainsKey(liveStreamId) && _CommentToDb[liveStreamId].Count >= 10)
                {
                    var comment = _CommentToDb[liveStreamId];
                    _CommentService.CreateComments(comment);
                    _CommentToDb[liveStreamId].RemoveRange(0, comment.Count());
                }
            }
        }

        public string RegexStreamIdInLink(string link)
        {
            // https://www.facebook.com/live/producer/3577219775726894/?entry_point=live_control_panel
            // https://www.facebook.com/phihung307/videos/3577219775726894
            if (link.Contains("videos"))
            {
                var s = link.Split("/");
                return s[s.Length - 1];
            }   
            Regex reg = new Regex(@"\d");   
            var id = "";
            foreach (Match item in reg.Matches(link))
            {
                id += item.ToString();
            }
            return id;
        }

        public string RegexVietNamPhone(string comment)
        {
            var phone = Regex.Match(comment, @"(0[3|9|8|5|7])([0-9]{8})").Value;
            return phone;
        }

        [HttpPost]
        public ActionResult GetComment(int liveStreamId)
        {
            try
            {
                var CommentStack = new Stack<Comment>();
                while (_Comments[_User.Id].Any())
                {
                    var comment = _Comments[_User.Id].Dequeue();
                    var phone = RegexVietNamPhone(comment.CommentContent);
                    if (phone != string.Empty)
                    {
                        comment.Phone = phone;
                    }
                    CommentStack.Push(comment);
                }
                var result = _CustomerService.GetCustomerForComment(CommentStack.ToList());
                return PartialView("_RowComment", result);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public ActionResult CreateOrderTemp(OrderTemp orderTemp, Customer customer, OrderDetail orderDetail)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                customer.UserId = _User.StoreId ?? 0;
                err = _CustomerService.CreateCustomer(customer);
                if (err.Code == Error.SUCCESS.Code)
                {
                    orderTemp.CustomerId = err.GetData<Customer>().Id;
                    err = _OrderService.CreateOrderDetail(orderDetail);
                    if (err.Code == Error.SUCCESS.Code)
                    {
                        orderTemp.OrderDetailId = err.GetData<OrderDetail>().Id;
                        orderTemp.IsPreSale = false;
                        err = _OrderService.CreateOrderTemp(orderTemp);
                    }
                }
                return Json(err);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(err.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, orderTemp, customer, orderDetail));
            }
        }

        // ------------------------------------------------------------------------


        public IActionResult ListOrderTemp(int id)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                ViewBag.LiveStream = _LiveStreamService.GetLiveStreamById(id);
                ViewBag.CurrentTimeZoneOffSet = _Timezoneoffset;
                return View();
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(err.System(ex));
            }
        }
        [HttpPost]
        public ActionResult DeleteOrderTemp(int id)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                err = _OrderService.DeleteOrderTempById(id);
                return Json(err);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(err.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id));
            }
        }
        public ActionResult RevertOrderTemp(int id)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                err = _OrderService.RevertOrderTempById(id);
                return Json(err);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, id));
            }
        }

        public IActionResult ChotDon(int id)
        {
            var err = new ErrorObject(Error.SUCCESS);
            try
            {
                var error = new ErrorObject(Error.SUCCESS);
                if (_LiveStreamService.CheckIsStreamming(id))
                {
                    var listComment = _CommentInLiveStream.Where(x => x.Value == id).Select(x => x.Key).ToList();
                    error = _OrderService.CreateOrder(id);
                    _LiveStreamService.UpdateIsStreamingLiveStream(id, null, false);
                    Task.Run(() =>
                    {
                        if (_CommentToDb.ContainsKey(id) && _CommentToDb[id].Count> 0)
                        {
                            _CommentService.CreateComments(_CommentToDb[id]);
                            _CommentToDb[id].Clear();
                        }
                    });
                }
                if (error.Code == Error.SUCCESS.Code)
                {
                    ViewBag.LiveStream = _LiveStreamService.GetLiveStreamById(id);
                    ViewBag.CurrentTimeZoneOffSet = _Timezoneoffset;
                    return View();
                }
                return RedirectToAction("Comments", new { id = id });
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(err.System(ex));
            }
        }

        [HttpPost]
        public PartialViewResult GetListOrderByLiveStream(SearchOrderFilter searchOrderFilter)
        {
            try
            {
                var totalRow = 0;
                var totalPage = 0;
                var orders = _OrderService.GetListOrderByIdLiveStream(searchOrderFilter);

                if (orders.Any())
                {
                    if (searchOrderFilter.AddressId != 999)
                    {
                        totalRow = _OrderService.CountTotalRowOrderByIdLiveStream(searchOrderFilter.IdLiveStream, searchOrderFilter.AddressId);
                    }
                    else
                    {
                        totalRow = orders.FirstOrDefault().TotalRow.Value;
                    }
                    totalPage = (int)Math.Ceiling((float)totalRow / searchOrderFilter.Top);
                }
                ViewBag.TotalRow = totalRow;
                ViewBag.TotalPage = totalPage;
                ViewBag.Page = searchOrderFilter.Page;
                return PartialView("_PartialListChotDon", orders);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public PartialViewResult GetListOrderTempByLiveStream(SearchOrderTempFilter search)
        {
            try
            {
                var totalRow = 0;
                var totalPage = 0;
                search.StartDate = TimeZoneInfo.ConvertTimeToUtc(search.StartDate, TimeZoneInfo.Local);
                search.EndDate = TimeZoneInfo.ConvertTimeToUtc(search.EndDate, TimeZoneInfo.Local);
                var orderTemps = _OrderService.GetListOrderTempByFilter(search);
                if (orderTemps.Any())
                {
                    {
                        totalRow = _OrderService.CountCustomerPreSaleByStatus(search.IdLiveStream, search.Status);
                    }
                    totalPage = (int)Math.Ceiling((float)totalRow / search.Top);
                }
                ViewBag.TotalRow = totalRow;
                ViewBag.TotalPage = totalPage;
                ViewBag.Page = search.Page;
                return PartialView("_PartialListOrderTemp", orderTemps);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return PartialView("_PartialError", ex);
            }
        }

        [HttpPost]
        public IActionResult CreateOrder(int idLiveStream)
        {
            var error = new ErrorObject(Error.SUCCESS);
            try
            {
                // create
                error = _OrderService.CreateOrder(idLiveStream);
                return Json(error);
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                return Json(error.System(ex));
            }
            finally
            {
                var method = MethodBase.GetCurrentMethod();
                _Log.Info(_LogService.WriteLog(method, idLiveStream));
            }
        }

        public FileResult ExportExcelOrder(int IdLiveStream, int AddressId)
        {
            try
            {
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                string fileName = "Order-" + DateTime.Now.ToString("dd/MM/yyyy") + ".xlsx";

                SearchOrderFilter searchOrderFilter = new SearchOrderFilter();
                searchOrderFilter.IdLiveStream = IdLiveStream;
                searchOrderFilter.AddressId = AddressId;
                if (AddressId != 999)
                {
                    searchOrderFilter.Top = _OrderService.CountTotalRowOrderByIdLiveStream(searchOrderFilter.IdLiveStream, searchOrderFilter.AddressId);
                }
                else
                {
                    searchOrderFilter.Top = _OrderService.GetListOrderByIdLiveStream(searchOrderFilter).FirstOrDefault().TotalRow.Value;
                }
                var orders = _OrderService.GetListOrderByIdLiveStream(searchOrderFilter);

                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Orders");
                    worksheet.Column(3).Style.NumberFormat.Format = "@";

                    worksheet.Cell(1, 1).Value = "STT";
                    worksheet.Cell(1, 2).Value = "Tên Facebook";
                    worksheet.Cell(1, 3).Value = "Số Điện Thoại";
                    worksheet.Cell(1, 4).Value = "Mã Sản Phẩm";
                    worksheet.Cell(1, 5).Value = "Giá";
                    worksheet.Cell(1, 6).Value = "Số Lượng";
                    worksheet.Cell(1, 7).Value = "Tổng Giá";

                    int row = 1;
                    int? soluongthieu = 0;
                    int? soluongconlai = 0;
                    double? totalPrice = 0;
                    foreach (var order in orders)
                    {
                        worksheet.Cell(row + 1, 1).Value = order.Row;
                        worksheet.Cell(row + 1, 2).Value = order.CustomerName;
                        worksheet.Cell(row + 1, 3).Value = order.CustomerPhone;
                        foreach (var orderDetail in order.OrderDetail)
                        {
                            worksheet.Cell(row + 1, 4).Value = orderDetail.Product.Code;
                            worksheet.Cell(row + 1, 5).Value = orderDetail.Price;
                            if (orderDetail.SoLuongProductConLai < 0)
                            {
                                soluongthieu += orderDetail.SoLuongProductConLai;
                                soluongconlai = orderDetail.Quantity + orderDetail.SoLuongProductConLai;
                                if (soluongconlai != 0)
                                {
                                    totalPrice += (orderDetail.Price * soluongconlai);
                                }
                                if (soluongconlai == 0)
                                {
                                    worksheet.Cell(row + 1, 6).Value = orderDetail.Quantity + " (Hết hàng)";
                                }
                                else
                                {
                                    worksheet.Cell(row + 1, 6).Value = orderDetail.Quantity + " (Kho còn " + soluongconlai + ")";
                                }
                            }
                            else
                            {
                                totalPrice += (orderDetail.Price * orderDetail.Quantity);
                                soluongconlai = orderDetail.Quantity;
                                worksheet.Cell(row + 1, 6).Value = orderDetail.Quantity;
                            }
                            worksheet.Cell(row + 1, 7).Value = orderDetail.Price * soluongconlai;
                            row++;
                        }
                        worksheet.Cell(row + 1, 6).Value = "Tổng số lượng: " + (order.TotalItem + soluongthieu);
                        worksheet.Cell(row + 1, 7).Value = "Tổng giá tiền: " + totalPrice;
                        soluongthieu = 0;
                        totalPrice = 0;
                        row++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw ex;
            }
        }

        public FileResult ExportExcelOrderTemp(SearchOrderTempFilter search)
        {
            try
            {
                search.StartDate = search.StartDate.AddSeconds(-int.Parse(_Timezoneoffset));
                search.EndDate = search.EndDate.AddSeconds(-int.Parse(_Timezoneoffset));
                string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                var dateTime = DateTime.Now;
                string fileName = "OrderPreSale-" + DateTime.Now.ToString("dd/MM/yyyy") + ".xlsx";
                var orderTemps = _OrderService.GetListOrderTempToExportExcel(search);
                using (var workbook = new XLWorkbook())
                {
                    IXLWorksheet worksheet = workbook.Worksheets.Add("Orders");
                    worksheet.Column(2).Style.NumberFormat.Format = "@";

                    worksheet.Cell(1, 1).Value = "Ngày tạo";
                    worksheet.Cell(1, 2).Value = "SĐT";
                    worksheet.Cell(1, 3).Value = "Tên khách hàng";
                    worksheet.Cell(1, 4).Value = "Category";
                    worksheet.Cell(1, 5).Value = "Cân nặng";
                    worksheet.Cell(1, 6).Value = "Số Lượng";
                    worksheet.Cell(1, 7).Value = "Giá sản phẩm";
                    worksheet.Cell(1, 8).Value = "Mã sản phẩm";
                    worksheet.Cell(1, 9).Value = "Tên sản phẩm";
                    worksheet.Cell(1, 10).Value = "Count";

                    int row = 1;
                    foreach (var orderTemp in orderTemps)
                    {
                        var createDate = orderTemp.LiveStream.CreatedDateUtc?.AddSeconds(int.Parse(_Timezoneoffset)).ToString().Substring(0, 10).Trim().Split("/");
                        worksheet.Cell(row + 1, 1).Value = createDate[1] + "/" + createDate[0] + "/" + createDate[2];
                        worksheet.Cell(row + 1, 2).Value = orderTemp.Customer.Phone;
                        worksheet.Cell(row + 1, 3).Value = orderTemp.Customer.Fullname;
                        worksheet.Cell(row + 1, 4).Value = orderTemp.OrderDetail.Product.ProductInfo.Category.Name;
                        worksheet.Cell(row + 1, 5).Value = orderTemp.OrderDetail.Product.Weight;
                        worksheet.Cell(row + 1, 6).Value = orderTemp.OrderDetail.Quantity;
                        worksheet.Cell(row + 1, 7).Value = orderTemp.OrderDetail.Price;
                        worksheet.Cell(row + 1, 8).Value = orderTemp.OrderDetail.Product.Code;
                        worksheet.Cell(row + 1, 9).Value = orderTemp.OrderDetail.Product.ProductInfo.Name;
                        worksheet.Cell(row + 1, 10).Value = orderTemp.Count;
                        row++;
                    }

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, contentType, fileName);
                    }
                }
            }
            catch (Exception ex)
            {
                _Log.Error(ex);
                throw ex;
            }
        }



    }
}
