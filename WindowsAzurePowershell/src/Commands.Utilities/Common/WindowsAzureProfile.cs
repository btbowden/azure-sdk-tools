﻿// ----------------------------------------------------------------------------------
//
// Copyright Microsoft Corporation
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// http://www.apache.org/licenses/LICENSE-2.0
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Commands.Utilities.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Properties;

    /// <summary>
    /// This class is the entry point for all the persistent
    /// state related to azure connections, including
    /// current environment, subscriptions, etc.
    /// </summary>
    public class WindowsAzureProfile
    {
        // Internal state

        // Store - responsible for loading and saving a profile to a particular location
        private readonly IProfileStore profileStore;

        // Azure environments
        private readonly Dictionary<string, WindowsAzureEnvironment> environments = new Dictionary<string, WindowsAzureEnvironment>(
            WindowsAzureEnvironment.PublicEnvironments, StringComparer.OrdinalIgnoreCase);

        // Func used to create the default instance
        private static readonly Func<WindowsAzureProfile> defaultCreator =
            () => new WindowsAzureProfile(new PowershellProfileStore());

        // Singleton instance management
        private static Lazy<WindowsAzureProfile> instance =
            new Lazy<WindowsAzureProfile>(defaultCreator);


        public WindowsAzureProfile(IProfileStore profileStore)
        {
            this.profileStore = profileStore;
            Load();
        }

        /// <summary>
        /// The default instance shared across the process.
        /// </summary>
        public static WindowsAzureProfile Instance
        {
            get
            {
                return instance.Value;
            }

            set
            {
                instance = new Lazy<WindowsAzureProfile>(() => value);
            }
        }
        
        /// <summary>
        /// Reset the default instance, used when the instance has been replaced for testing.
        /// </summary>
        public static void ResetInstance()
        {
            instance = new Lazy<WindowsAzureProfile>(defaultCreator);
        }

        //
        // Azure environments
        //

        public IDictionary<string, WindowsAzureEnvironment> Environments { 
            get { return new Dictionary<string, WindowsAzureEnvironment>(environments, StringComparer.OrdinalIgnoreCase); }
        }

        private WindowsAzureEnvironment currentEnvironment;

        public WindowsAzureEnvironment CurrentEnvironment
        {
            get
            {
                if (currentEnvironment == null)
                {
                    currentEnvironment = environments[EnvironmentName.AzureCloud];
                }
                return currentEnvironment;
            }

            set
            {
                if (!environments.ContainsKey(value.Name))
                {
                    AddEnvironment(value);
                }
                currentEnvironment = environments[value.Name];

                Save();
            }
        }

        public void AddEnvironment(WindowsAzureEnvironment newEnvironment)
        {
            if (environments.ContainsKey(newEnvironment.Name))
            {
                throw new InvalidOperationException(string.Format(Resources.EnvironmentExists, newEnvironment.Name));
            }

            environments[newEnvironment.Name] = newEnvironment;

            Save();
        }

        public void UpdateEnvironment(WindowsAzureEnvironment newEnvironment)
        {
            GuardExistsAndNonPublic(newEnvironment.Name);
            environments[newEnvironment.Name] = newEnvironment;
            Save();
        }

        public void RemoveEnvironment(string name)
        {
            GuardExistsAndNonPublic(name);
            environments.Remove(name);
            Save();
        }

        private void GuardExistsAndNonPublic(string name)
        {
            if (IsPublicEnvironment(name))
            {
                throw new InvalidOperationException(string.Format(Resources.ChangePublicEnvironmentMessage, name));
            }
            if (!environments.ContainsKey(name))
            {
                throw new KeyNotFoundException(string.Format(Resources.EnvironmentNotFound, name));
            }
        }

        private void Load()
        {
            if (profileStore != null)
            {
                var profileData = profileStore.Load();
                if (profileData != null)
                {
                    LoadEnvironmentData(profileData);
                }
            }
        }

        private void Save()
        {
            var profileData = new ProfileData();
            SetEnvironmentData(profileData);
            profileStore.Save(profileData);
        }

        private static bool IsPublicEnvironment(string name)
        {
            return
                WindowsAzureEnvironment.PublicEnvironments.Keys.Any(
                    publicName => string.Compare(publicName, name, StringComparison.OrdinalIgnoreCase) == 0);
        }

        private void SetEnvironmentData(ProfileData data)
        {
            data.DefaultEnvironmentName = CurrentEnvironment.Name;
            data.Environments = Environments.Values
                .Where(e => !IsPublicEnvironment(e.Name))
                .Select(e => new AzureEnvironmentData(e));
        }

        private void LoadEnvironmentData(ProfileData data)
        {
            if (data.Environments != null)
            {
                foreach (var e in data.Environments.Select(e => e.ToAzureEnvironment()))
                {
                    environments[e.Name] = e;
                }
                if (environments.ContainsKey(data.DefaultEnvironmentName))
                {
                    currentEnvironment = environments[data.DefaultEnvironmentName];
                }
            }
        }
    }
}
