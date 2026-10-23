using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.BookingViewModels
{
    public class MemberForSessionViewModel
    {
        public int MemberId { get; set; }
        public int SessionId { get; set; }
        public string MemberName { get; set; } = string.Empty;
        public string BookingDate { get; set; } = string.Empty;
        public bool IsAttended { get; set; }
    }
}
