namespace Repair.Web.Mng.Models
{
    /// <summary>
    /// 消息类型
    /// </summary>
    public enum InfoType
    {
        /// <summary>
        /// 提示信息
        /// </summary>
        Info,
        /// <summary>
        /// 警告信息
        /// </summary>
        Warning,
        /// <summary>
        /// 错误信息
        /// </summary>
        Error,
        /// <summary>
        /// 严重错误
        /// </summary>
        Danger,
        /// <summary>
        /// 成功信息
        /// </summary>
        Success
    }

    /// <summary>
    /// 确认对话框
    /// </summary>
    public class DlgConfirmInfo
    {
        /// <summary>
        /// 初始化
        /// </summary>
        public DlgConfirmInfo()
        {
            Type = InfoType.Info;
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 消息类型
        /// </summary>
        public InfoType Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}