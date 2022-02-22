using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gif2Spritesheet.Services
{
    public interface IFolderDialogService : IDialogService
    {
        public string Filter { get; set; }
        public string InitialDirectory { get; set; }
        public string Folder { get; set; }
    }
}
