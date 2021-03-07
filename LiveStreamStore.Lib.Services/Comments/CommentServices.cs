using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Comments
{
    public class CommentServices:ICommentService
    {
        public ErrorObject CreateComments(List<Comment> comments)
        {
            var error = Error.Success();
            try
            {
                using (var context = new LiveStreamStoreContext())
                {
                    foreach (var item in comments)
                    {
                        context.Comment.Add(item);
                    }
                    return context.SaveChanges() > 0 ? error : error.Failed("Failed");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public List<Comment> GetCommentByLiveStreamId(int LiveStreamId)
        {
            try
            {
                var comments = new List<Comment>();
                using (var context = new LiveStreamStoreContext())
                {
                    comments = context.Comment.Where(x => x.LiveStreamId == LiveStreamId).ToList();
                    return comments;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
