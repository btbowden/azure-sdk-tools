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

namespace Microsoft.WindowsAzure.Commands.MediaServices
{
    using System;
    using Utilities.Common;

    public class AzureMediaServicesHttpClientCommandBase : CmdletBase
    {
        private SubscriptionData _currentSubscription;

        /// <summary>
        ///     Gets or sets the current subscription.
        /// </summary>
        /// <value>
        ///     The current subscription.
        /// </value>
        public SubscriptionData CurrentSubscription
        {
            get
            {
                if (_currentSubscription == null)
                {
                    _currentSubscription = this.GetCurrentSubscription();
                }

                return _currentSubscription;
            }

            set
            {
                if (_currentSubscription != value)
                {
                    _currentSubscription = value;
                }
            }
        }

        protected virtual void OnProcessRecord()
        {
            // Intentionally left blank
        }
        protected override void ProcessRecord()
        {
            try
            {
                Validate.ValidateInternetConnection();
                ExecuteCmdlet();
                OnProcessRecord();
            }
            catch (Exception ex)
            {
                WriteExceptionError(ex);
            }
        }
        protected static void CatchAggregatedExceptionFlattenAndRethrow(Action c)
        {
            try
            {
                c();
            }
            catch (AggregateException ex)
            {
                var flat = ex.Flatten();
                if (flat.InnerExceptions.Count == 1)
                {
                    throw flat.InnerException;
                }
                else
                {
                    throw flat;
                }
            }
        }
    }
}