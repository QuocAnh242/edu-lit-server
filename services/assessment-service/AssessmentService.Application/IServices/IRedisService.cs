namespace AssessmentService.Application.IServices;

    /// <summary>
    /// Mở rộng ICacheService với các tính năng đặc thù của Redis
    /// như Sorted Sets và Pub/Sub.
    /// </summary>
    public interface IRedisService : ICacheService
    {
        // ===== Advanced: Sorted Set (for pagination / ranking) =====

        /// <summary>
        /// Thêm một thành viên vào một tập hợp đã sắp xếp (sorted set) với một điểm số.
        /// </summary>
        /// <param name="key">Key của sorted set.</param>
        /// <param name="member">Giá trị của thành viên cần thêm.</param>
        /// <param name="score">Điểm số dùng để sắp xếp thành viên.</param>
        Task AddToSortedSetAsync(string key, string member, double score);

        /// <summary>
        /// Lấy một danh sách các thành viên từ một sorted set trong một khoảng nhất định.
        /// </summary>
        /// <param name="key">Key của sorted set.</param>
        /// <param name="start">Chỉ số bắt đầu (0-based index).</param>
        /// <param name="stop">Chỉ số kết thúc. Sử dụng -1 để lấy đến cuối cùng.</param>
        /// <param name="descending">True để sắp xếp theo thứ tự giảm dần, false để tăng dần.</param>
        /// <returns>Một danh sách các thành viên trong khoảng đã cho.</returns>
        Task<IEnumerable<string>> GetRangeFromSortedSetAsync(string key, long start, long stop, bool descending = true);

        /// <summary>
        /// Lấy tổng số lượng thành viên trong một sorted set.
        /// </summary>
        /// <param name="key">Key của sorted set.</param>
        /// <returns>Số lượng thành viên trong set.</returns>
        Task<long> GetSortedSetLengthAsync(string key);

        /// <summary>
        /// Xóa một thành viên cụ thể khỏi sorted set.
        /// </summary>
        /// <param name="key">Key của sorted set.</param>
        /// <param name="member">Thành viên cần xóa.</param>
        Task RemoveFromSortedSetAsync(string key, string member);

        /// <summary>
        /// Xóa toàn bộ một sorted set.
        /// </summary>
        /// <param name="key">Key của sorted set cần xóa.</param>
        Task ClearSortedSetAsync(string key);

        // ===== Pub/Sub (optional) =====

        /// <summary>
        /// Gửi (publish) một tin nhắn đến một kênh (channel) cụ thể.
        /// </summary>
        /// <param name="channel">Tên của kênh để gửi tin nhắn.</param>
        /// <param name="message">Nội dung tin nhắn.</param>
        Task PublishAsync(string channel, string message);

        /// <summary>
        /// Đăng ký (subscribe) để lắng nghe tin nhắn từ một kênh.
        /// </summary>
        /// <param name="channel">Tên của kênh cần lắng nghe.</param>
        /// <param name="handler">Hàm sẽ được gọi để xử lý tin nhắn nhận được.</param>
        void Subscribe(string channel, Action<string> handler);
    }