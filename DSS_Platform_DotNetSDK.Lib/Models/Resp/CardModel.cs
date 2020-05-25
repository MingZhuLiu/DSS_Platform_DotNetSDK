using System;
namespace DSS_Platform_DotNetSDK.Lib.Models.Resp {
    public class CardModel {
        public long? id { get; set; }
        public string cardNumber { get; set; }
        public string cardType { get; set; }
        public string category { get; set; }
        public string cardStatus { get; set; }
        public string startDate { get; set; }
        public string endDate { get; set; }
        public string cardPassword { get; set; }
        public string subSystems { get; set; }
        public string cardSubsidy { get; set; }

        public string cardDeposit { get; set; }
        public string cardCash { get; set; }
        public string cardCost { get; set; }

        public long? availableTimes { get; set; }
        public long? balance { get; set; }
        public long? cardFormatedTotalBalance { get; set; }

        public string[] cardIds { get; set; }
        public long? cardMachine { get; set; }

        // public bool @checked { get; set; }
        // public bool chkDisabled { get; set; }

        public string createTime { get; set; }
        public long? deptId { get; set; }

        public long? isMainCard { get; set; }

        public long? personId { get; set; }
        public string personName { get; set; }
        public long? personIdentityId { get; set; }

        public string validFlag { get; set; }

    }
}