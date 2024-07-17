using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PDTrader.Library;

namespace PDTrader
{
    internal class Transaction // only handing one way selling right now, no active trading
    {
        public int ID { get; private set; } = 0;
        public string Name { get; private set; } = "";
        public TransactionStatus Status = TransactionStatus.Greeting;
        public Item Wants = Item.Undecided;
        public int WantsQuantity = 0;
        public Item Has = Item.Undecided;
        public int HasQuantity = 0;

        private Transaction(int _id, string _username)
        {
            this.ID = _id;
            this.Name = _username;
        }

        public static Transaction New(int _id, string _username) => new Transaction(_id, _username);
    }
}
