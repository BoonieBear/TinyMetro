using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TinyMetroWpfLibrary.Frames;

namespace TinyMetroWpfLibrary.Events
{
    public class ChangeAnimationModeRequest
    {
        /// <summary>
        /// Initializes a new instance of the ChangeAnimationModeRequest request
        /// </summary>
        /// <param name="animationMode"></param>
        public ChangeAnimationModeRequest(AnimationMode animationMode)
        {
            AnimationMode = animationMode;
        }

        /// <summary>
        /// Gets the animation mode
        /// </summary>
        public AnimationMode AnimationMode  { get; set; }
    }
}
