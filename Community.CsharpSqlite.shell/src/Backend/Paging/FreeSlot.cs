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
        public int NextAddress { get; protected set; }

        public FreeSlot pNext
        {
            get
            {
                return NextAddress > 0 ? new FreeSlot(page, NextAddress) : null;
            }

            set
            {
                if (null == value)
                {
                    NextAddress = 0;
                }
                else {
                    data[Address + 0] = data[value.Address + 0];
                    data[Address + 1] = data[value.Address + 1];
                }
            }
        }
    }
}
