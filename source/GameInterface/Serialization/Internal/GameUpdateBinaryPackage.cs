using GameInterface.Services.Modules;
using GameInterface.Services.Updater;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameInterface.Serialization.Internal
{
    public class GameUpdateBinaryPackage : BinaryPackageBase<GameUpdateData>
    {
        public GameUpdateBinaryPackage(GameUpdateData obj, IBinaryPackageFactory binaryPackageFactory) : base(obj, binaryPackageFactory)
        {
        }

        protected override void PackInternal()
        {
            base.PackFields();
        }

        protected override void UnpackInternal()
        {
            base.UnpackFields();
        }
    }
}
