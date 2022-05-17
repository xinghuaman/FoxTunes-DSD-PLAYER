﻿using FoxTunes.Interfaces;
using System.Collections.Generic;

namespace FoxTunes
{
    public static class TaskbarThumbnailBehaviourConfiguration
    {
        public const string SECTION = WindowsConfiguration.SECTION;

        public const string ENABLED_ELEMENT = "CCCC32FB-CC7A-43C7-8C3D-525DDDBD12AA";

        public static IEnumerable<ConfigurationSection> GetConfigurationSections()
        {
            var releaseType = StandardComponents.Instance.Configuration.ReleaseType;
            yield return new ConfigurationSection(SECTION, Strings.WindowsConfiguration_Section)
                .WithElement(new BooleanConfigurationElement(ENABLED_ELEMENT, Strings.TaskbarThumbnailBehaviourConfiguration_Enabled).WithValue(releaseType == ReleaseType.Default)
            );
        }
    }
}
