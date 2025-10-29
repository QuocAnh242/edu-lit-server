namespace AssessmentService.Application.IServices;

    /// <summary>
    /// Định nghĩa các hoạt động cache key-value cơ bản,
    /// có thể áp dụng cho mọi nhà cung cấp cache (ví dụ: Redis, MemoryCache, Memcached).
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Thiết lập giá trị cho một key trong cache.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu của giá trị cần lưu.</typeparam>
        /// <param name="key">Key định danh cho dữ liệu.</param>
        /// <param name="value">Giá trị cần lưu.</param>
        /// <param name="expiry">Thời gian tồn tại của cache. Nếu là null, cache sẽ không tự động hết hạn.</param>
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// Lấy giá trị từ cache dựa vào key.
        /// </summary>
        /// <typeparam name="T">Kiểu dữ liệu mong muốn khi lấy ra.</typeparam>
        /// <param name="key">Key của dữ liệu cần lấy.</param>
        /// <returns>
        /// Trả về giá trị của key dưới dạng đối tượng kiểu T.
        /// Trả về null nếu key không tồn tại.
        /// </returns>
        Task<T?> GetAsync<T>(string key);

        /// <summary>
        /// Xóa một key và giá trị tương ứng khỏi cache.
        /// </summary>
        /// <param name="key">Key cần xóa.</param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Kiểm tra xem một key có tồn tại trong cache hay không.
        /// </summary>
        /// <param name="key">Key cần kiểm tra.</param>
        /// <returns>Trả về true nếu key tồn tại, ngược lại trả về false.</returns>
        Task<bool> ExistsAsync(string key);
    }