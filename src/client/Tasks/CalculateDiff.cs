using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDS.FileSystem;

namespace CDS.Tasks
{
    public class CalculateDiff : Task2
    {
        private DirectoryEntry _left, _right;

        public CalculateDiff(DirectoryEntry left, DirectoryEntry right)
        {
            _left = left;
            _right = right;
        }

        protected override void Execute()
        {
            //request commit lock (return some sort of GUID with 2 timed lock)
            
            //compute changes using blockingcollection

            //send files as we get them (send GUID to refresh commit lock timer)
            
            //send commit complete (send guid to notice that we are done)
        }


    }
}
