﻿// ----------------------------------------------------------------------------------
//
// Copyright 2011 Microsoft Corporation
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

namespace Microsoft.WindowsAzure.Management.Websites.Services.Github
{
    using Microsoft.WindowsAzure.Management.Websites.Services.Github.Entities;
    using System.Collections.Generic;
    using WebEntities;

    public static class GithubExtensionMethods
    {
        public static IList<GithubOrganization> GetOrganizations(this IGithubServiceManagement proxy)
        {
            return proxy.EndGetOrganizations(proxy.BeginGetOrganizations(null, null));
        }

        public static IList<GithubOrganization> GetOrganizationsFromUser(this IGithubServiceManagement proxy, string user)
        {
            return proxy.EndGetOrganizationsFromUser(proxy.BeginGetOrganizationsFromUser(user, null, null));
        }

        public static IList<GithubRepository> GetRepositories(this IGithubServiceManagement proxy)
        {
            return proxy.EndGetRepositories(proxy.BeginGetRepositories(null, null));
        }

        public static IList<GithubRepository> GetRepositoriesFromUser(this IGithubServiceManagement proxy, string user)
        {
            return proxy.EndGetRepositoriesFromUser(proxy.BeginGetRepositoriesFromUser(user, null, null));
        }

        public static IList<GithubRepository> GetRepositoriesFromOrg(this IGithubServiceManagement proxy, string organization)
        {
            return proxy.EndGetRepositoriesFromOrg(proxy.BeginGetRepositoriesFromOrg(organization, null, null));
        }

        public static IList<GithubRepositoryHook> GetRepositoryHooks(this IGithubServiceManagement proxy, string owner, string repository)
        {
            return proxy.EndGetRepositoryHooks(proxy.BeginGetRepositoryHooks(owner, repository, null, null));
        }

        public static void CreateRepositoryHook(this IGithubServiceManagement proxy, string owner, string repository, GithubRepositoryHook hook)
        {
            proxy.EndCreateRepositoryHook(proxy.BeginCreateRepositoryHook(owner, repository, hook, null, null));
        }

        public static void UpdateRepositoryHook(this IGithubServiceManagement proxy, string owner, string repository, string id, GithubRepositoryHook hook)
        {
            proxy.EndUpdateRepositoryHook(proxy.BeginUpdateRepositoryHook(owner, repository, id, hook, null, null));
        }

        public static void TestRepositoryHook(this IGithubServiceManagement proxy, string owner, string repository, string id)
        {
            proxy.EndTestRepositoryHook(proxy.BeginTestRepositoryHook(owner, repository, id, null, null));
        }
    }
}
