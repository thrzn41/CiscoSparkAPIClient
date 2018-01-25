/* 
 * MIT License
 * 
 * Copyright(c) 2017 thrzn41
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Thrzn41.CiscoSpark.Version1.Admin
{

    /// <summary>
    /// Cisco Spark Admin API Client for API version 1.
    /// </summary>
    public class SparkAdminAPIClient : SparkAPIClient
    {

        /// <summary>
        /// Spark organizations API Path.
        /// </summary>
        protected static readonly string SPARK_ORGANIZATION_API_PATH = GetAPIPath("organizations");

        /// <summary>
        /// Spark licenses API Path.
        /// </summary>
        protected static readonly string SPARK_LICENSE_API_PATH = GetAPIPath("licenses");

        /// <summary>
        /// Spark roles API Path.
        /// </summary>
        protected static readonly string SPARK_ROLE_API_PATH = GetAPIPath("roles");

        /// <summary>
        /// Spark events API Path.
        /// </summary>
        protected static readonly string SPARK_EVENT_API_PATH = GetAPIPath("events");
        

        /// <summary>
        /// Spark organizations API Uri.
        /// </summary>
        protected static readonly Uri SPARK_ORGANIZATION_API_URI = new Uri(SPARK_ORGANIZATION_API_PATH);

        /// <summary>
        /// Spark licenses API Uri.
        /// </summary>
        protected static readonly Uri SPARK_LICENSE_API_URI = new Uri(SPARK_LICENSE_API_PATH);

        /// <summary>
        /// Spark roles API Uri.
        /// </summary>
        protected static readonly Uri SPARK_ROLE_API_URI = new Uri(SPARK_ROLE_API_PATH);

        /// <summary>
        /// Spark events API Uri.
        /// </summary>
        protected static readonly Uri SPARK_EVENT_API_URI = new Uri(SPARK_EVENT_API_PATH);




        /// <summary>
        /// Constructor of SparkAdminAPIClient.
        /// </summary>
        /// <param name="token">token of Spark API.</param>
        internal SparkAdminAPIClient(string token)
            : base(token)
        {
        }




        /// <summary>
        /// Gets value or default.
        /// </summary>
        /// <typeparam name="TResult">Type of the value.</typeparam>
        /// <param name="value">Value.</param>
        /// <param name="defaultValue">Default value that is returned if value parameter is null.</param>
        /// <returns>Value or default.</returns>
        private static TResult getValueOrDefault<TResult>(TResult value, TResult defaultValue)
        {
            return ((value != null) ? value : defaultValue);
        }




        #region Person APIs

        /// <summary>
        /// Creates a person.
        /// </summary>
        /// <param name="email">Email addresses of the person.</param>
        /// <param name="displayName">Full name of the person.</param>
        /// <param name="firstName">First name of the person.</param>
        /// <param name="lastName">Last name of the person.</param>
        /// <param name="avatarUri">URL to the person's avatar in PNG format.</param>
        /// <param name="organizationId">ID of the organization to which this person belongs.</param>
        /// <param name="roles">Roles of the person</param>
        /// <param name="licenses">Licenses allocated to the person</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Person> > CreatePersonAsync(string email, string displayName = null, string firstName = null, string lastName = null, Uri avatarUri = null, string organizationId = null, IEnumerable<string> roles = null, IEnumerable<string> licenses = null, CancellationToken? cancellationToken = null)
        {
            List<string> roleList    = null;
            List<string> licenseList = null;

            if(roles != null)
            {
                roleList = new List<string>();

                foreach (var item in roles)
                {
                    roleList.Add(item);
                }
            }

            if (licenses != null)
            {
                licenseList = new List<string>();

                foreach (var item in licenses)
                {
                    licenseList.Add(item);
                }
            }


            var person = new Person();

            person.Emails         = new[] { email };
            person.DisplayName    = displayName;
            person.FirstName      = firstName;
            person.LastName       = lastName;
            person.Avatar         = avatarUri?.AbsoluteUri;
            person.OrganizationId = organizationId;
            person.Roles          = roleList?.ToArray();
            person.Licenses       = licenseList?.ToArray();

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Person>, Person>(
                                    HttpMethod.Post,
                                    SPARK_PERSON_API_URI,
                                    null,
                                    person,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Updates a person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to be updated.</param>
        /// <param name="email">Email addresses of the person.</param>
        /// <param name="displayName">Full name of the person.</param>
        /// <param name="firstName">First name of the person.</param>
        /// <param name="lastName">Last name of the person.</param>
        /// <param name="avatarUri">URL to the person's avatar in PNG format.</param>
        /// <param name="organizationId">ID of the organization to which this person belongs.</param>
        /// <param name="roles">Roles of the person</param>
        /// <param name="licenses">Licenses allocated to the person</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Person> > UpdatePersonAsync(Person person, string email = null, string displayName = null, string firstName = null, string lastName = null, Uri avatarUri = null, string organizationId = null, IEnumerable<string> roles = null, IEnumerable<string> licenses = null, CancellationToken? cancellationToken = null)
        {
            List<string> roleList    = null;
            List<string> licenseList = null;

            if (roles != null)
            {
                roleList = new List<string>();

                foreach (var item in roles)
                {
                    roleList.Add(item);
                }
            }

            if (licenses != null)
            {
                licenseList = new List<string>();

                foreach (var item in licenses)
                {
                    licenseList.Add(item);
                }
            }


            var updatedPerson = new Person();

            updatedPerson.Emails         = new[] { getValueOrDefault(email, person.Emails[0]) };
            updatedPerson.DisplayName    = getValueOrDefault(displayName,            person.DisplayName);
            updatedPerson.FirstName      = getValueOrDefault(firstName,              person.FirstName);
            updatedPerson.LastName       = getValueOrDefault(lastName,               person.LastName);
            updatedPerson.Avatar         = getValueOrDefault(avatarUri?.AbsoluteUri, person.Avatar);
            updatedPerson.OrganizationId = getValueOrDefault(organizationId,         person.OrganizationId);
            updatedPerson.Roles          = getValueOrDefault(roleList?.ToArray(),    person.Roles);
            updatedPerson.Licenses       = getValueOrDefault(licenseList?.ToArray(), person.Licenses);

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Person>, Person>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", SPARK_PERSON_API_PATH, Uri.EscapeDataString(person.Id))),
                                    null,
                                    updatedPerson,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Deletes a person.
        /// </summary>
        /// <param name="personId">Person id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeletePersonAsync(string personId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", SPARK_PERSON_API_PATH, Uri.EscapeDataString(personId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a person.
        /// </summary>
        /// <param name="person"><see cref="Person"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task<SparkResult<NoContent>> DeletePersonAsync(Person person, CancellationToken? cancellationToken = null)
        {
            return (await DeletePersonAsync(person.Id, cancellationToken));
        }

        #endregion


        #region Organization APIs

        /// <summary>
        /// Lists organizations. 
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<OrganizationList> > ListOrganizationsAsync(CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<OrganizationList>, OrganizationList>(
                                    HttpMethod.Get,
                                    SPARK_ORGANIZATION_API_URI,
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets organization detail.
        /// </summary>
        /// <param name="organizationId">Organization id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Organization> > GetOrganizationAsync(string organizationId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Organization>, Organization>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_ORGANIZATION_API_PATH, Uri.EscapeDataString(organizationId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets organization detail.
        /// </summary>
        /// <param name="organization"><see cref="Organization"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task<SparkResult<Organization>> GetOrganizationAsync(Organization organization, CancellationToken? cancellationToken = null)
        {
            return (await GetOrganizationAsync(organization.Id, cancellationToken));
        }

        #endregion


        #region License APIs

        /// <summary>
        /// Lists licenses. 
        /// </summary>
        /// <param name="organizationId">Specify the organization.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<LicenseList> > ListLicensesAsync(string organizationId, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("orgId", organizationId);

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<LicenseList>, LicenseList>(
                                    HttpMethod.Get,
                                    SPARK_LICENSE_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists licenses. 
        /// </summary>
        /// <param name="organization">Specify the <see cref="Organization"/>.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<LicenseList> > ListLicensesAsync(Organization organization, CancellationToken? cancellationToken = null)
        {
            return (await ListLicensesAsync(organization.Id, cancellationToken));
        }


        /// <summary>
        /// Gets license detail.
        /// </summary>
        /// <param name="licenseId">License id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<License> > GetLicenseAsync(string licenseId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<License>, License>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_LICENSE_API_PATH, Uri.EscapeDataString(licenseId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets license detail.
        /// </summary>
        /// <param name="license"><see cref="License"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<License> > GetLicenseAsync(License license, CancellationToken? cancellationToken = null)
        {
            return (await GetLicenseAsync(license.Id, cancellationToken));
        }

        #endregion


        #region Role APIs

        /// <summary>
        /// Lists Roles. 
        /// </summary>
        /// <param name="max">Limit the maximum number of entries in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<RoleList> > ListRolesAsync(int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("max", max?.ToString());

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<RoleList>, RoleList>(
                                    HttpMethod.Get,
                                    SPARK_ROLE_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets role detail.
        /// </summary>
        /// <param name="roleId">Role id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Role> > GetRoleAsync(string roleId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Role>, Role>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_ROLE_API_PATH, Uri.EscapeDataString(roleId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets role detail.
        /// </summary>
        /// <param name="role"><see cref="Role"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Role> > GetRoleAsync(Role role, CancellationToken? cancellationToken = null)
        {
            return (await GetRoleAsync(role.Id, cancellationToken));
        }

        #endregion


        #region Event APIs

        /// <summary>
        /// Lists Events. 
        /// </summary>
        /// <param name="resource">Limit results to a specific resource type.</param>
        /// <param name="type">Limit results to a specific event type.</param>
        /// <param name="actorId">Limit results to events performed by this person, by ID.</param>
        /// <param name="from">Limit results to events which occurred after a date and time.</param>
        /// <param name="to">Limit results to events which occurred before a date and time.</param>
        /// <param name="max">Limit the maximum number of entries in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<EventDataList> > ListEventsAsync(EventResource resource = null, EventType type = null, string actorId = null, DateTime? from = null, DateTime? to = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("resource", resource?.Name);
            queryParameters.Add("type",     type?.Name);
            queryParameters.Add("actorId",  actorId);
            queryParameters.Add("from",     from?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"));
            queryParameters.Add("to",       to?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"));
            queryParameters.Add("max",      max?.ToString());

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<EventDataList>, EventDataList>(
                                    HttpMethod.Get,
                                    SPARK_EVENT_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets event detail.
        /// </summary>
        /// <param name="eventId">Event id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<EventData> > GetEventAsync(string eventId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<EventData>, EventData>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_EVENT_API_PATH, Uri.EscapeDataString(eventId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets event detail.
        /// </summary>
        /// <param name="eventData"><see cref="EventData"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<EventData> > GetEventAsync(EventData eventData, CancellationToken? cancellationToken = null)
        {
            return (await GetEventAsync(eventData.Id, cancellationToken));
        }

        #endregion

    }

}
