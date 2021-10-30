using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DDMusic.Areas.Admin.API.Model
{
    public class ApiCode
    {
        /// <summary>
        /// Thành công
        /// </summary>
        public static readonly string SUCCESS = "SUCCESS";
        /// <summary>
        /// Lỗi không xác định, thử lại sau
        /// </summary>
        public static readonly string FATAL_ERROR = "FATAL_ERROR";
        /// <summary>
        /// Đơn hàng đã tồn tại
        /// </summary>
        public static readonly string IS_READY_EXISTED = "IS_READY_EXISTED";
        /// <summary>
        /// Dữ liệu đầu vào không hợp lệ
        /// </summary>
        public static readonly string INVALID_INPUT_DATA = "INVALID_INPUT_DATA";
        /// <summary>
        /// Dữ liệu yêu cầu không được tìm thấy
        /// </summary>
        public static readonly string NOT_FOUND = "NOT_FOUND";
        /// <summary>
        /// Vi phạm ràng buộc cơ sở dữ liệu
        /// </summary>
        public static readonly string CONSTRAINT_VIOLATED = "CONSTRAINT_VIOLATED";
    }
}
