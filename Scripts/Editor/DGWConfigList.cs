//=====================================================================================================================
//
// ideMobi copyright 2020 
// All rights reserved by ideMobi
//
//=====================================================================================================================

using System;
using System.Collections.Generic;

//=====================================================================================================================
namespace DoxygenGeneratorWindow
{
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
    /// <summary>
    /// DGW config List reccord your config!
    /// </summary>
    [Serializable]
    public class DGWConfigList
    {
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// The List.
        /// </summary>
        public List<DGWConfig> ConfigList = new List<DGWConfig>();
        /// <summary>
        /// The selected Config
        /// </summary>
        public int Selected = 0;
        //-------------------------------------------------------------------------------------------------------------
        /// <summary>
        /// Constructor
        /// </summary>
        public DGWConfigList()
        {
        }
        //-------------------------------------------------------------------------------------------------------------
        public DGWConfig AddNewConfig()
        {
            DGWConfig tConfig = new DGWConfig();
            tConfig.EmptyFill();
            Selected = ConfigList.Count;
            ConfigList.Add(tConfig);
            return tConfig;
        }
        //-------------------------------------------------------------------------------------------------------------
        public DGWConfig RemoveConfig(DGWConfig tConfig)
        {
            tConfig.EmptyFill();
            ConfigList.Remove(tConfig);
            Selected = 0;
            DGWConfig rConfig = ConfigList[Selected];
            if (ConfigList.Count == 0)
            {
                rConfig = new DGWConfig();
                rConfig.EmptyFill();
                ConfigList.Add(rConfig);
            }
            return rConfig;
        }
        //-------------------------------------------------------------------------------------------------------------
    }
    //+++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
}
//=====================================================================================================================