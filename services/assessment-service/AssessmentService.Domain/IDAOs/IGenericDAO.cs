using AssessmentService.Domain.Commons;
using System.Linq.Expressions;

namespace AssessmentService.Domain.IDAOs
{
    public interface IGenericDAO<T> where T : class
    {
        /// <summary>
        /// Lấy một đối tượng theo khóa chính (ID).
        /// </summary>
        /// <param name="id">Khóa chính của đối tượng.</param>
        /// <returns>Trả về đối tượng nếu tìm thấy, ngược lại trả về null.</returns>
        Task<T?> GetByIdAsync(int id);

        /// <summary>
        /// Lấy tất cả các đối tượng của một bảng.
        /// </summary>
        /// <returns>Danh sách tất cả các đối tượng.</returns>
        Task<List<T>> GetAllAsync();

        // Optional: You can add a method to get entities by a specific condition
        Task<List<T>> GetAllByAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Lấy một danh sách các đối tượng đã được phân trang.
        /// </summary>
        /// <param name="pageIndex">Chỉ số của trang (bắt đầu từ 1).</param>
        /// <param name="pageSize">Số lượng mục trên mỗi trang.</param>
        /// <param name="filter"></param>
        /// <returns>Kết quả phân trang chứa danh sách các mục và thông tin phân trang.</returns>
        Task<PagedResult<T>> GetPagedAsync(int pageIndex, int pageSize, Expression<Func<T, bool>>? filter = null);

        /// <summary>
        /// Thêm một đối tượng mới.
        /// </summary>
        /// <param name="entity">Đối tượng cần thêm.</param>
        Task AddAsync(T entity);

        /// <summary>
        /// Thêm nhiều đối tượng mới.
        /// </summary>
        /// <param name="entities">Danh sách đối tượng cần thêm.</param>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Đánh dấu một đối tượng là đã bị thay đổi.
        /// </summary>
        /// <param name="entity">Đối tượng cần cập nhật.</param>
        void Update(T entity);

        /// <summary>
        /// Đánh dấu một đối tượng sẽ bị xóa.
        /// </summary>
        /// <param name="entity">Đối tượng cần xóa.</param>
        void Remove(T entity);
    }
}
