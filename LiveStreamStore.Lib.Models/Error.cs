using System;
using System.Collections.Generic;
using System.Text;

namespace LiveStreamStore.Lib.Models
{
    [Serializable]
    public class Error
    {
        public static ErrorObject SUCCESS = new ErrorObject { Code = 0, Message = "Thành công." };
        public static ErrorObject FAILED = new ErrorObject { Code = 1, Message = "Thất bại." };
        public static ErrorObject SYSTEM = new ErrorObject { Code = 2, Message = "Lỗi hệ thống." };
        public static ErrorObject DATABASE = new ErrorObject { Code = 3, Message = "Lỗi database." };
        public static ErrorObject AUTH = new ErrorObject { Code = 4, Message = "Lỗi chứng thực." };
        public static ErrorObject INVALID_DATA = new ErrorObject { Code = 5, Message = "Dữ liệu vào không chính xác." };

        //Các lỗi về LiveStream
        public static ErrorObject LIVESTREAM_LINK_EXISTED = new ErrorObject { Code = 6, Message = "Link đã tồn tại." };

        //Các lỗi về Product
        public static ErrorObject PRODUCT_NAME_EXISTED = new ErrorObject { Code = 7, Message = "Tên sản phẩm đã tồn tại." };
        public static ErrorObject PRODUCT_CODE_EXISTED = new ErrorObject { Code = 8, Message = "Mã sản phẩm đã tồn tại." };
        public static ErrorObject CHECK_PRODUCT = new ErrorObject { Code = 9, Message = "Số lượng còn lại không đủ hoặc đã bán hết." };

        //Các lỗi về UploadFile
        public static ErrorObject INCORRECT_IMAGE_FORMAT = new ErrorObject { Code = 113, Message = "Định dạng hình ảnh không chính xác! Vui lòng chọn ảnh khác." };
        public static ErrorObject IMAGE_SIZE_LARGE = new ErrorObject { Code = 114, Message = "Kích thước hình ảnh quá lớn! Vui lòng chọn ảnh khác." };

        //Các lỗi về Customer
        public static ErrorObject CUSTOMER_INVALID = new ErrorObject { Code = 120, Message = "Tài khoản hoặc mật khẩu không chính xác." };
        public static ErrorObject CUSTOMER_EMAIL_EXISTED = new ErrorObject { Code = 121, Message = "Địa chỉ email đã được sử dụng." };
        public static ErrorObject PHONE_EXISTED = new ErrorObject { Code = 122, Message = "Số điện thoại đã được sử dụng." };
        public static ErrorObject PASSWORD_INCORRECT = new ErrorObject { Code = 123, Message = "Mật khẩu cũ không chính xác." };

        //Các lỗi về ShoppingCart
        public static ErrorObject LIMITED = new ErrorObject { Code = 124, Message = "Số lượng bạn thêm vượt quá giới hạn được mua." };

        //vì các biến trên là static nên dẫn đến bị setData chồng lên nhau
        //=> tạo ra các function khác vùng nhớ
        //Dùng khi có kết quả trả về 
        public static ErrorObject Get(ErrorObject ErrorObj, object Data = null)
        {
            return new ErrorObject(ErrorObj, Data);
        }

        public static ErrorObject Success()
        {
            return new ErrorObject(SUCCESS);
        }

    }

    [Serializable]
    public class ErrorObject
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; }
        public ErrorObject(int Code, string Message, object Data = null)
        {
            this.Code = Code;
            this.Message = Message;
            this.Data = Data;
        }

        //set mặc định thành công
        public ErrorObject()
        {
            Code = 0;
            Message = "Thành công";
        }

        public ErrorObject(ErrorObject Obj)
        {
            this.Code = Obj.Code;
             this.Message = Obj.Message;
            this.Data = Obj.Data;
        }

        public ErrorObject(ErrorObject Obj, object Data)
        {
            this.Code = Obj.Code;
            this.Message = Obj.Message;
            this.Data = Data;
        }

        public ErrorObject SetCode(ErrorObject Obj)
        {
            this.Code = Obj.Code;
            this.Message = Obj.Message;
            return this;
        }

        public ErrorObject SetCode(ErrorObject Obj, object Data)
        {
            SetCode(Obj).Data = Data;
            return this;
        }

        public ErrorObject SetMessage(string Message)
        {
            this.Message = Message;
            return this;
        }

        public ErrorObject SetData(object Data)
        {
            this.Data = Data;
            return this;
        }

        public object GetData()
        {
            return this.Data;
        }

        public ErrorObject Success(object Data)
        {
            return SetCode(Error.SUCCESS, Data);
        }

        public ErrorObject Failed(object Data)
        {
            return SetCode(Error.FAILED, Data);
        }

        public ErrorObject Failed(string Message)
        {
            return SetCode(Error.FAILED).SetMessage(Message);
        }

        public ErrorObject System(object Data)
        {
            return SetCode(Error.SYSTEM, Data);
        }

        public T GetData<T>()
        {
            return (T)Data;

        }
    }
}
