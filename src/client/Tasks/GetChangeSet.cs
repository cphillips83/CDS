using CDS.FileSystem;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDS.Tasks
{
    public class GetChangeSet : Task2
    {
        private List<ChangeEntry> _localChangeSet = new List<ChangeEntry>();
        //private List<ChangeEntry> _localChangeSet2 = new List<ChangeEntry>();

        public readonly BlockingCollection<ChangeEntry> Changes = new BlockingCollection<ChangeEntry>();
        public readonly DirectoryEntry Left;
        public readonly DirectoryEntry Right;
        public bool FoundChanges { get; private set; } = false;


        public GetChangeSet(DirectoryEntry left, DirectoryEntry right)
        {
            Left = left;
            Right = right;
        }

        protected override void Execute()
        {
            FoundChanges = DirectoryEntry.FindChanges(Left, Right, x =>
            {
                Changes.Add(x);
                lock (_localChangeSet)
                    _localChangeSet.Add(x);

            });
        }

        private void WriteChanges()
        {
            ChangeEntry[] entries = null;
            lock (_localChangeSet)
            {
                if (_localChangeSet.Count > 0)
                {
                    entries = _localChangeSet.ToArray();
                    _localChangeSet.Clear();
                }
            }

            if (entries != null)
            {
                foreach (var cs in entries)
                    Console.WriteLine(cs.GetChangeLog());
            }
        }

        public override bool Wait(int ms = -1)
        {
            while (!base.Wait(Globals.ConsoleUpdateDelay))
                WriteChanges();

            WriteChanges();
            return true;
        }
    }
}
