using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CDS.FileSystem;
using System.Collections.Concurrent;

namespace CDS.Tasks
{
    public class GetChangeSet : Task2
    {
        private DirectoryEntry _left, _right;

        public readonly BlockingCollection<ChangeEntry> Changes = new BlockingCollection<ChangeEntry>();

        public GetChangeSet(DirectoryEntry left, DirectoryEntry right)
        {
            _left = left;
            _right = right;
        }

        protected override void Execute()
        {
            //request commit lock (return some sort of GUID with 2 timed lock)

            //compute changes using blockingcollection
            CompareDirectories(_left, _right);

            //send files as we get them (send GUID to refresh commit lock timer)

            //send commit complete (send guid to notice that we are done)


        }

        private void CompareDirectories(DirectoryEntry left, DirectoryEntry right)
        {
            CompareFiles(left, right);

            var lefthash = left != null ?  new HashSet<string>(left.Directories.Keys) : new HashSet<string>();
            var righthash = right != null ?  new HashSet<string>(right.Directories.Keys) : new HashSet<string>();
            var bothhash = new HashSet<string>(lefthash);
            foreach (var key in righthash)
                bothhash.Add(key);

            foreach (var dir in bothhash)
            {
                var sdleft = left != null ? left.Directories.SafeGet(dir) : null;
                var sdright = right != null ? right.Directories.SafeGet(dir) : null;

                var result = DirectoryEntry.Compare(sdleft, sdright);
                if (result != ChangeType.OK)
                    Changes.Add(new ChangeEntry(dir, result, true));

                if (result != ChangeType.Delete)
                    CompareDirectories(sdleft, sdright);
            }
        }

        private void CompareFiles(DirectoryEntry left, DirectoryEntry right)
        {
            var hash = new HashSet<string>();

            if (left != null)
            {
                foreach (var fe in left.Files.Keys)
                    hash.Add(fe);
            }

            if (right != null)
            {
                foreach (var fe in right.Files.Keys)
                    hash.Add(fe);
            }

            foreach (var file in hash)
            {
                var fileleft = left != null ? left.Files.SafeGet(file) : null;
                var fileright = right != null ? right.Files.SafeGet(file) : null;
                var result = FileEntry.Compare(fileleft, fileright);

                if (result != ChangeType.OK)
                    Changes.Add(new ChangeEntry(file, result, false));
            }
        }
    }
}
