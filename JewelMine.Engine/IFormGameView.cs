using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JewelMine.Engine
{
    /// <summary>
    /// Represents a windows form game view contract.
    /// </summary>
    public interface IFormGameView : IGameView, IDisposable
    {
        /// <summary>
        /// Centers the window.
        /// </summary>
        void CenterWindow();

        /// <summary>
        /// Gets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        Form Window { get; }
    }
}
