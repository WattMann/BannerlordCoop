﻿using GameInterface.Helpers;

namespace GameInterface
{
    public class GameInterface : IGameInterface
    {
        public IExampleGameHelper ExampleGameHelper { get; }

        public ISaveLoadHelper SaveLoadHelper { get; }

        public GameInterface(
            IExampleGameHelper exampleGameHelper,
            ISaveLoadHelper saveLoadHelper)
        {
            ExampleGameHelper = exampleGameHelper;
            SaveLoadHelper = saveLoadHelper;
        }
    }
}
