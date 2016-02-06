using Community.CsharpSqlite.Utils;
using u16 = System.UInt16;


namespace Community.CsharpSqlite.Paging
{
    public class FreeSlot : ILinkedListNode<FreeSlot>
    {
        MemPage page;
        byte[] data;
        public FreeSlot(MemPage page, int address)
            : this(page, address, (u16)page.aData.get2byte(address + 2))
        {

        }
        public FreeSlot(MemPage page, int address, u16 size)
        {
            this.page = page;
            this.data = page.aData;
            this._size = size;
            this.Address = address;
            this.NextAddress = (u16)data.get2byte(address);
        }
        u16 _size;
        public u16 Size
        {
            get { return _size; }
            set { data.put2byte(this.Address + 2, _size = value); }
        }

        public int Address { get; protected set; }
        int _nextAddress;
        public int NextAddress { get { return _nextAddress; } protected set { data.put2byte(this.Address , _nextAddress = value); } }

        public FreeSlot pNext
        {
            get
            {
                return 0==NextAddress  ? null:new FreeSlot(page, NextAddress) ;
            }

            set
            {
                NextAddress = null==value?0:value.Address;
            }
        }
    }
}
