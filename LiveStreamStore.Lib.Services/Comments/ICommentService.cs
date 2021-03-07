using System;
using System.Collections.Generic;
using System.Text;
using LiveStreamStore.Lib.Data.DBContext.Models;
using LiveStreamStore.Lib.Models;

namespace LiveStreamStore.Lib.Services.Comments
{
    public interface ICommentService
    {
        ErrorObject CreateComments(List<Comment> comments);
        List<Comment> GetCommentByLiveStreamId(int LiveStreamId);

    }
}
