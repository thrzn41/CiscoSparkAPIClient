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
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Thrzn41.Util;

namespace Thrzn41.CiscoSpark.Version1
{

    /// <summary>
    /// Cisco Spark API Client for API version 1.
    /// </summary>
    public class SparkAPIClient : IDisposable
    {

        /// <summary>
        /// Spark API Path.
        /// </summary>
        protected const string SPARK_API_PATH = "https://api.ciscospark.com/v1/";


        /// <summary>
        /// Spark person API Path.
        /// </summary>
        protected static readonly string SPARK_PERSON_API_PATH = getAPIPath("people");

        /// <summary>
        /// Spark spaces API Path.
        /// </summary>
        protected static readonly string SPARK_SPACES_API_PATH = getAPIPath("rooms");

        /// <summary>
        /// Spark memberships API Path.
        /// </summary>
        protected static readonly string SPARK_SPACE_MEMBERSHIPS_API_PATH = getAPIPath("memberships");

        /// <summary>
        /// Spark messages API Path.
        /// </summary>
        protected static readonly string SPARK_MESSAGES_API_PATH = getAPIPath("messages");

        /// <summary>
        /// Spark teams API Path.
        /// </summary>
        protected static readonly string SPARK_TEAMS_API_PATH = getAPIPath("teams");

        /// <summary>
        /// Spark team memberships API Path.
        /// </summary>
        protected static readonly string SPARK_TEAM_MEMBERSHIPS_API_PATH = getAPIPath("team/memberships");

        /// <summary>
        /// Spark webhooks API Path.
        /// </summary>
        protected static readonly string SPARK_WEBHOOKS_API_PATH = getAPIPath("webhooks");


        /// <summary>
        /// Spark person API Uri.
        /// </summary>
        protected static readonly Uri SPARK_PERSON_API_URI = new Uri(SPARK_PERSON_API_PATH);

        /// <summary>
        /// Spark messages API Uri.
        /// </summary>
        protected static readonly Uri SPARK_SPACES_API_URI = new Uri(SPARK_SPACES_API_PATH);

        /// <summary>
        /// Spark memberships API Uri.
        /// </summary>
        protected static readonly Uri SPARK_SPACE_MEMBERSHIPS_API_URI = new Uri(SPARK_SPACE_MEMBERSHIPS_API_PATH);

        /// <summary>
        /// Spark messages API Uri.
        /// </summary>
        protected static readonly Uri SPARK_MESSAGES_API_URI = new Uri(SPARK_MESSAGES_API_PATH);

        /// <summary>
        /// Spark teams API Uri.
        /// </summary>
        protected static readonly Uri SPARK_TEAMS_API_URI = new Uri(SPARK_TEAMS_API_PATH);

        /// <summary>
        /// Spark team memberships API Uri.
        /// </summary>
        protected static readonly Uri SPARK_TEAM_MEMBERSHIPS_API_URI = new Uri(SPARK_TEAM_MEMBERSHIPS_API_PATH);

        /// <summary>
        /// Spark webhooks API Uri.
        /// </summary>
        protected static readonly Uri SPARK_WEBHOOKS_API_URI = new Uri(SPARK_WEBHOOKS_API_PATH);


        /// <summary>
        /// Uri pattern of Spark API.
        /// </summary>
        private readonly static Regex SPARK_API_URI_PATTERN = new Regex(String.Format("^{0}", SPARK_API_PATH), RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));

        /// <summary>
        /// Person email pattern.
        /// This regex pattern is Not intended to detect exact email pattern.
        /// This is a vague pattern intentionally.
        /// </summary>
        private readonly static Regex SPARK_PERSON_EMAIL_PATTERN = new Regex("^[^@]+@[^@]+$", RegexOptions.Compiled, TimeSpan.FromSeconds(60.0f));


        /// <summary>
        /// Random generator.
        /// </summary>
        private readonly static CryptoRandom RAND = new CryptoRandom();



        /// <summary>
        /// HttpClient for Spark API.
        /// </summary>
        protected readonly SparkHttpClient sparkHttpClient;




        /// <summary>
        /// Constructor of SparkAPIClient.
        /// </summary>
        /// <param name="token">token of Spark API.</param>
        internal SparkAPIClient(string token)
        {
            this.sparkHttpClient = new SparkHttpClient(token, SPARK_API_URI_PATTERN);
        }


        /// <summary>
        /// Gets API path for each api.
        /// </summary>
        /// <param name="apiPath">Each api path.</param>
        /// <returns>Full path for the api.</returns>
        protected static string getAPIPath(string apiPath)
        {
            return String.Format("{0}{1}", SPARK_API_PATH, apiPath);
        }


        /// <summary>
        /// Detects person id type.
        /// </summary>
        /// <param name="personId">Person id to be detected.</param>
        /// <param name="personIdType">Person id type to be detected.</param>
        /// <returns>Detected <see cref="PersonIdType"/>.</returns>
        internal static PersonIdType DetectPersonIdType(string personId, PersonIdType personIdType)
        {
            var result = personIdType;

            if(personIdType == PersonIdType.Detect)
            {
                if( SPARK_PERSON_EMAIL_PATTERN.IsMatch(personId) )
                {
                    result = PersonIdType.Email;
                }
                else
                {
                    result = PersonIdType.Id;
                }
            }

            return result;
        }


        /// <summary>
        /// Builds comma separated string.
        /// </summary>
        /// <param name="values">Value list.</param>
        /// <returns>Comma separated string.</returns>
        internal static string BuildCommaSeparatedString(IEnumerable<string> values)
        {
            if(values == null)
            {
                return null;
            }

            var strs = new StringBuilder();

            var separator = String.Empty;

            foreach (var item in values)
            {
                if(item != null)
                {
                    strs.AppendFormat("{0}{1}", separator, item);
                    separator = ",";
                }
            }

            return strs.ToString();
        }


        #region Person APIs

        /// <summary>
        /// Lists people.
        /// </summary>
        /// <param name="email">List people with this email address. For non-admin requests, either this or displayName are required.</param>
        /// <param name="displayName">List people whose name starts with this string. For non-admin requests, either this or email are required.</param>
        /// <param name="ids">List people by ID. Accepts up to 85 person IDs.</param>
        /// <param name="max">Limit the maximum number of people in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<PersonList> > ListPersonsAsync(string email = null, string displayName = null, IEnumerable<string> ids = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("email",       email);
            queryParameters.Add("displayName", displayName);
            queryParameters.Add("id",          BuildCommaSeparatedString(ids));
            queryParameters.Add("max",         max?.ToString());

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<PersonList>, PersonList>(
                                    HttpMethod.Get,
                                    SPARK_PERSON_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists people.
        /// </summary>
        /// <param name="email">List people with this email address. For non-admin requests, either this or displayName are required.</param>
        /// <param name="displayName">List people whose name starts with this string. For non-admin requests, either this or email are required.</param>
        /// <param name="ids">List people by ID. Accepts up to 85 person IDs.</param>
        /// <param name="max">Limit the maximum number of people in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<PersonList> > ListPeopleAsync(string email = null, string displayName = null, IEnumerable<string> ids = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (await ListPersonsAsync(email, displayName, ids, max, cancellationToken));
        }


        /// <summary>
        /// Get person detail.
        /// </summary>
        /// <param name="personId">Person id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Person> > GetPersonAsync(string personId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Person>, Person>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_PERSON_API_URI, Uri.EscapeDataString(personId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Get person detail.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Person> > GetPersonAsync(Person person, CancellationToken? cancellationToken = null)
        {
            return (await GetPersonAsync(person.Id, cancellationToken));
        }


        /// <summary>
        /// Get my own detail.
        /// </summary>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Person> > GetMeAsync(CancellationToken? cancellationToken = null)
        {
            return (await GetPersonAsync("me", cancellationToken));
        }

        #endregion


        #region Spaces APIs

        /// <summary>
        /// Lists spaces. 
        /// </summary>
        /// <param name="teamId">Limit the rooms to those associated with a team, by ID.</param>
        /// <param name="type"><see cref="SpaceType.Direct"/> returns all 1-to-1 rooms. <see cref="SpaceType.Group"/> returns all group rooms.If not specified or values are not matched, will return all room types.</param>
        /// <param name="sortBy">Sort results by space ID(<see cref="SpaceSortBy.Id"/>), most recent activity(<see cref="SpaceSortBy.LastActivity"/>), or most recently created(<see cref="SpaceSortBy.Created"/>).</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<SpaceList> > ListSpacesAsync(string teamId = null, SpaceType type = null, SpaceSortBy sortBy = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("teamId", teamId);
            queryParameters.Add("max",    max?.ToString());
            queryParameters.Add("type",   type?.Name);
            queryParameters.Add("sortBy", sortBy?.Name);

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<SpaceList>, SpaceList>(
                                    HttpMethod.Get,
                                    SPARK_SPACES_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Create a space.
        /// </summary>
        /// <param name="title">A user-friendly name for the room.</param>
        /// <param name="teamId">The ID for the team with which this room is associated.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Space> > CreateSpaceAsync(string title, string teamId = null, CancellationToken? cancellationToken = null)
        {
            var space = new Space();

            space.Title  = title;
            space.TeamId = teamId;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Space>, Space>(
                                    HttpMethod.Post,
                                    SPARK_SPACES_API_URI,
                                    null,
                                    space,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets space detail.
        /// </summary>
        /// <param name="spaceId">Space id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Space> > GetSpaceAsync(string spaceId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Space>, Space>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_SPACES_API_PATH, Uri.EscapeDataString(spaceId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets space detail.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Space> > GetSpaceAsync(Space space, CancellationToken? cancellationToken = null)
        {
            return (await GetSpaceAsync(space.Id, cancellationToken));
        }


        /// <summary>
        /// Updates space.
        /// </summary>
        /// <param name="spaceId">Space id to be updated.</param>
        /// <param name="title">A user-friendly name for the space.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Space> > UpdateSpaceAsync(string spaceId, string title, CancellationToken? cancellationToken = null)
        {
            var space = new Space();

            space.Title = title;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Space>, Space>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", SPARK_SPACES_API_PATH, Uri.EscapeDataString(spaceId))),
                                    null,
                                    space,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates space.
        /// </summary>
        /// <param name="space"><see cref="Space"/> to be updated.</param>
        /// <param name="title">A user-friendly name for the space.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Space> > UpdateSpaceAsync(Space space, string title, CancellationToken? cancellationToken = null)
        {
            return (await UpdateSpaceAsync(space.Id, title, cancellationToken));
        }


        /// <summary>
        /// Deletes a space.
        /// </summary>
        /// <param name="spaceId">Space id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteSpaceAsync(string spaceId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", SPARK_SPACES_API_PATH, Uri.EscapeDataString(spaceId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a space.
        /// </summary>
        /// <param name="space"><see cref="Space"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteSpaceAsync(Space space, CancellationToken? cancellationToken = null)
        {
            return (await DeleteSpaceAsync(space.Id, cancellationToken));
        }

        #endregion


        #region SpaceMemberships APIs

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="spaceId">Limit results to a specific space, by ID.</param>
        /// <param name="personIdOrEmail">Limit results to a specific person, by ID or Email.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(string spaceId = null, string personIdOrEmail = null, int? max = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            string personIdOrEmailKey;

            switch (personIdType)
            {
                case PersonIdType.Email:
                    personIdOrEmailKey = "personEmail";
                    break;
                default:
                    personIdOrEmailKey = "personId";
                    break;
            }

            var queryParameters = new NameValueCollection();

            queryParameters.Add("roomId",           spaceId);
            queryParameters.Add(personIdOrEmailKey, personIdOrEmail);
            queryParameters.Add("max",              max?.ToString());

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<SpaceMembershipList>, SpaceMembershipList>(
                                    HttpMethod.Get,
                                    SPARK_SPACE_MEMBERSHIPS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="space">Limit results to a specific <see cref="Space"/>.</param>
        /// <param name="person">Limit results to a specific <see cref="Person"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(Space space, Person person = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (await ListSpaceMembershipsAsync(space.Id, person?.Id, max, PersonIdType.Id, cancellationToken));
        }

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="space">Limit results to a specific <see cref="Space"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(Space space, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (await ListSpaceMembershipsAsync(space.Id, null, max, PersonIdType.Id, cancellationToken));
        }

        /// <summary>
        /// Lists space memberships.
        /// </summary>
        /// <param name="person">Limit results to a specific <see cref="Person"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<SpaceMembershipList> > ListSpaceMembershipsAsync(Person person, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (await ListSpaceMembershipsAsync(null, person.Id, max, PersonIdType.Id, cancellationToken));
        }


        /// <summary>
        /// Create a space membership.
        /// </summary>
        /// <param name="spaceId">The space ID.</param>
        /// <param name="personIdOrEmail">The person ID or Email.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SpaceMembership> > CreateSpaceMembershipAsync(string spaceId, string personIdOrEmail, bool? isModerator = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            var membership = new SpaceMembership();

            membership.SpaceId = spaceId;

            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            switch (personIdType)
            {
                case PersonIdType.Email:
                    membership.PersonEmail = personIdOrEmail;
                    break;
                default:
                    membership.PersonId = personIdOrEmail;
                    break;
            }

            membership.IsModerator = isModerator;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<SpaceMembership>, SpaceMembership>(
                                    HttpMethod.Post,
                                    SPARK_SPACE_MEMBERSHIPS_API_URI,
                                    null,
                                    membership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Create a space membership.
        /// </summary>
        /// <param name="space"><see cref="Space"/>.</param>
        /// <param name="person"><see cref="Person"/>.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SpaceMembership> > CreateSpaceMembershipAsync(Space space, Person person, bool? isModerator = null, CancellationToken? cancellationToken = null)
        {
            return (await CreateSpaceMembershipAsync(space.Id, person.Id, isModerator, PersonIdType.Id, cancellationToken));
        }


        /// <summary>
        /// Gets space membership detail.
        /// </summary>
        /// <param name="membershipId">Space Membership id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SpaceMembership> > GetSpaceMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<SpaceMembership>, SpaceMembership>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_SPACE_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets space membership detail.
        /// </summary>
        /// <param name="membership"><see cref="SpaceMembership"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SpaceMembership> > GetSpaceMembershipAsync(SpaceMembership membership, CancellationToken? cancellationToken = null)
        {
            return (await GetSpaceMembershipAsync(membership.Id, cancellationToken));
        }


        /// <summary>
        /// Updates space membership.
        /// </summary>
        /// <param name="membershipId">Membership id to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a space moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SpaceMembership> > UpdateSpaceMembershipAsync(string membershipId, bool isModerator, CancellationToken? cancellationToken = null)
        {
            var membership = new SpaceMembership();

            membership.IsModerator = isModerator;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<SpaceMembership>, SpaceMembership>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", SPARK_SPACE_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    membership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates space membership.
        /// </summary>
        /// <param name="membership"><see cref="SpaceMembership"/> to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a space moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SpaceMembership> > UpdateSpaceMembershipAsync(SpaceMembership membership, bool isModerator, CancellationToken? cancellationToken = null)
        {
            return (await UpdateSpaceMembershipAsync(membership.Id, isModerator, cancellationToken));
        }


        /// <summary>
        /// Deletes a space membership.
        /// </summary>
        /// <param name="membershipId">Space Membership id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteSpaceMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", SPARK_SPACE_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a space membership.
        /// </summary>
        /// <param name="membership"><see cref="SpaceMembership"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteSpaceMembershipAsync(SpaceMembership membership, CancellationToken? cancellationToken = null)
        {
            return (await DeleteSpaceMembershipAsync(membership.Id, cancellationToken));
        }

        #endregion


        #region Messages APIs

        /// <summary>
        /// Lists messages.
        /// </summary>
        /// <param name="spaceId">List messages for a space, by ID.</param>
        /// <param name="mentionedPeople">List messages where the caller is mentioned by specifying "me" or the caller personId.</param>
        /// <param name="before">List messages sent before a date and time.</param>
        /// <param name="beforeMessage">List messages sent before a message, by ID.</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<MessageList> > ListMessagesAsync(string spaceId, string mentionedPeople = null, DateTime? before = null, string beforeMessage = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("roomId",          spaceId);
            queryParameters.Add("max",             max?.ToString());
            queryParameters.Add("mentionedPeople", mentionedPeople);
            queryParameters.Add("before",          before?.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fffzzz"));
            queryParameters.Add("beforeMessage",   beforeMessage);

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<MessageList>, MessageList>(
                                    HttpMethod.Get,
                                    SPARK_MESSAGES_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists messages.
        /// </summary>
        /// <param name="space">List messages for <see cref="Space"/>.</param>
        /// <param name="mentionedPeople">List messages where the caller is mentioned by specifying "me" or the caller personId.</param>
        /// <param name="before">List messages sent before a date and time.</param>
        /// <param name="beforeMessage">List messages sent before a message, by ID.</param>
        /// <param name="max">Limit the maximum number of messages in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<MessageList> > ListMessagesAsync(Space space, string mentionedPeople = null, DateTime? before = null, string beforeMessage = null, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (await ListMessagesAsync(space.Id, mentionedPeople, before, beforeMessage, max, cancellationToken));
        }


        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="targetId">Id that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="target"><see cref="MessageTarget"/> that the targetId parameter represents.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateMessageAsync(string targetId, string markdownOrText, IEnumerable<Uri> files = null, MessageTarget target = MessageTarget.SpaceId, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            var message = new Message();

            switch (target)
            {
                case MessageTarget.PersonId:
                    message.ToPersonId = targetId;
                    break;
                case MessageTarget.PersonEmail:
                    message.ToPersonEmail = targetId;
                    break;
                default:
                    message.SpaceId = targetId;
                    break;
            }

            switch (textType)
            {
                case MessageTextType.Text:
                    message.Text = markdownOrText;
                    break;
                default:
                    message.Markdown = markdownOrText;
                    break;
            }

            if(files != null)
            {
                List<string> fileList = new List<string>();

                foreach (var item in files)
                {
                    fileList.Add(item.AbsoluteUri);
                }

                message.Files = fileList.ToArray();
            }

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Message>, Message>(
                                    HttpMethod.Post,
                                    SPARK_MESSAGES_API_URI,
                                    null,
                                    message,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateMessageAsync(Space space, string markdownOrText, IEnumerable<Uri> files = null, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (await CreateMessageAsync(space.Id, markdownOrText, files, MessageTarget.SpaceId, textType, cancellationToken));
        }


        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="targetId">Id that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="target"><see cref="MessageTarget"/> that the targetId parameter represents.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateMessageAsync(string targetId, string markdownOrText, SparkFileData fileData, MessageTarget target = MessageTarget.SpaceId, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            string targetKey;
            string textKey;

            switch (target)
            {
                case MessageTarget.PersonId:
                    targetKey = "toPersonId";
                    break;
                case MessageTarget.PersonEmail:
                    targetKey = "toPersonEmail";
                    break;
                default:
                    targetKey = "roomId";
                    break;
            }

            switch (textType)
            {
                case MessageTextType.Text:
                    textKey = "text";
                    break;
                default:
                    textKey = "markdown";
                    break;
            }

            var stringData = new NameValueCollection();

            stringData.Add(targetKey, targetId);
            stringData.Add(textKey,   markdownOrText);

            var result = await this.sparkHttpClient.RequestMultipartFormDataAsync<SparkResult<Message>, Message>(
                                    HttpMethod.Post,
                                    SPARK_MESSAGES_API_URI,
                                    null,
                                    stringData,
                                    fileData,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Creates a message.
        /// </summary>
        /// <param name="space"><see cref="Space"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateMessageAsync(Space space, string markdownOrText, SparkFileData fileData, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (await CreateMessageAsync(space.Id, markdownOrText, fileData, MessageTarget.SpaceId, textType, cancellationToken));
        }



        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="personIdOrEmail">Person id or email that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateDirectMessageAsync(string personIdOrEmail, string markdownOrText, IEnumerable<Uri> files = null, PersonIdType personIdType = PersonIdType.Detect, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            MessageTarget targetType;

            switch (personIdType)
            {
                case PersonIdType.Email:
                    targetType = MessageTarget.PersonEmail;
                    break;
                default:
                    targetType = MessageTarget.PersonId;
                    break;
            }

            return (await CreateMessageAsync(personIdOrEmail, markdownOrText, files, targetType, textType, cancellationToken));
        }

        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="personIdOrEmail">Person id or email that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> of personIdOrEmail parameter.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateDirectMessageAsync(string personIdOrEmail, string markdownOrText, SparkFileData fileData, PersonIdType personIdType = PersonIdType.Detect, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            MessageTarget targetType;

            switch (personIdType)
            {
                case PersonIdType.Email:
                    targetType = MessageTarget.PersonEmail;
                    break;
                default:
                    targetType = MessageTarget.PersonId;
                    break;
            }

            return (await CreateMessageAsync(personIdOrEmail, markdownOrText, fileData, targetType, textType, cancellationToken));
        }

        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="files">File uris to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateDirectMessageAsync(Person person, string markdownOrText, IEnumerable<Uri> files = null, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (await CreateDirectMessageAsync(person.Id, markdownOrText, files, PersonIdType.Id, textType, cancellationToken));
        }

        /// <summary>
        /// Create a message to direct space.
        /// </summary>
        /// <param name="person"><see cref="Person"/> that the message is posted.</param>
        /// <param name="markdownOrText">markdown or text to be posted.</param>
        /// <param name="fileData">File data to be attached with the message.</param>
        /// <param name="textType"><see cref="MessageTextType"/> of markdownOrText parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > CreateDirectMessageAsync(Person person, string markdownOrText, SparkFileData fileData, MessageTextType textType = MessageTextType.Markdown, CancellationToken? cancellationToken = null)
        {
            return (await CreateDirectMessageAsync(person.Id, markdownOrText, fileData, PersonIdType.Id, textType, cancellationToken));
        }


        /// <summary>
        /// Gets message detail.
        /// </summary>
        /// <param name="messageId">Message id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > GetMessageAsync(string messageId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Message>, Message>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_MESSAGES_API_PATH, Uri.EscapeDataString(messageId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets message detail from Cisco Spark.
        /// </summary>
        /// <param name="message"><see cref="Message"/> that the detail info is be gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Message> > GetMessageAsync(Message message, CancellationToken? cancellationToken = null)
        {
            return (await GetMessageAsync(message.Id, cancellationToken));
        }


        /// <summary>
        /// Deletes message from Cisco Spark.
        /// </summary>
        /// <param name="messageId">Message id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteMessageAsync(string messageId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", SPARK_MESSAGES_API_PATH, Uri.EscapeDataString(messageId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes message from Cisco Spark.
        /// </summary>
        /// <param name="message">Message to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task<SparkResult<NoContent>> DeleteMessageAsync(Message message, CancellationToken? cancellationToken = null)
        {
            return (await DeleteMessageAsync(message.Id, cancellationToken));
        }

        #endregion


        #region Teams APIs

        /// <summary>
        /// Lists teams.
        /// </summary>
        /// <param name="max">Limit the maximum number of teams in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<TeamList> > ListTeamsAsync(int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("max", max?.ToString());

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<TeamList>, TeamList>(
                                    HttpMethod.Get,
                                    SPARK_TEAMS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Creates a team.
        /// </summary>
        /// <param name="name">A user-friendly name for the team.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Team> > CreateTeamAsync(string name, CancellationToken? cancellationToken = null)
        {
            var team = new Team();

            team.Name = name;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Team>, Team>(
                                    HttpMethod.Post,
                                    SPARK_TEAMS_API_URI,
                                    null,
                                    team,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets team detail.
        /// </summary>
        /// <param name="teamId">Team id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Team> > GetTeamAsync(string teamId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Team>, Team>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_TEAMS_API_PATH, Uri.EscapeDataString(teamId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets team detail.
        /// </summary>
        /// <param name="team"><see cref="Team"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Team> > GetTeamAsync(Team team, CancellationToken? cancellationToken = null)
        {
            return (await GetTeamAsync(team.Id, cancellationToken));
        }


        /// <summary>
        /// Updates team info.
        /// </summary>
        /// <param name="teamId">Team id to be updated.</param>
        /// <param name="name">A user-friendly name for the team.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Team> > UpdateTeamAsync(string teamId, string name, CancellationToken? cancellationToken = null)
        {
            var team = new Team();

            team.Name = name;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Team>, Team>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", SPARK_TEAMS_API_PATH, Uri.EscapeDataString(teamId))),
                                    null,
                                    team,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates team info.
        /// </summary>
        /// <param name="team"><see cref="Team"/> to be updated.</param>
        /// <param name="name">A user-friendly name for the team.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task<SparkResult<Team>> UpdateTeamAsync(Team team, string name, CancellationToken? cancellationToken = null)
        {
            return (await UpdateTeamAsync(team.Id, name, cancellationToken));
        }

        /// <summary>
        /// Deletes a team.
        /// </summary>
        /// <param name="teamId">Team id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteTeamAsync(string teamId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", SPARK_TEAMS_API_PATH, Uri.EscapeDataString(teamId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a team.
        /// </summary>
        /// <param name="team"><see cref="Team"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteTeamAsync(Team team, CancellationToken? cancellationToken = null)
        {
            return (await DeleteTeamAsync(team.Id, cancellationToken));
        }


        #endregion


        #region TeamMemberships APIs

        /// <summary>
        /// Lists team memberships.
        /// </summary>
        /// <param name="teamId">List team memberships for a team, by ID.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<TeamMembershipList> > ListTeamMembershipsAsync(string teamId, int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("teamId", teamId);
            queryParameters.Add("max",    max?.ToString());

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<TeamMembershipList>, TeamMembershipList>(
                                    HttpMethod.Get,
                                    SPARK_TEAM_MEMBERSHIPS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Lists team memberships.
        /// </summary>
        /// <param name="team">List team memberships for <see cref="Team"/>.</param>
        /// <param name="max">Limit the maximum number of items in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task<SparkListResult<TeamMembershipList>> ListTeamMembershipsAsync(Team team, int? max = null, CancellationToken? cancellationToken = null)
        {
            return (await ListTeamMembershipsAsync(team.Id, max, cancellationToken));
        }


        /// <summary>
        /// Create a team membership.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="personIdOrEmail">The person ID or Email.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="personIdType"><see cref="PersonIdType"/> for personIdOrEmail parameter.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<TeamMembership> > CreateTeamMembershipAsync(string teamId, string personIdOrEmail, bool? isModerator = null, PersonIdType personIdType = PersonIdType.Detect, CancellationToken? cancellationToken = null)
        {
            var teamMembership = new TeamMembership();

            teamMembership.TeamId = teamId;

            personIdType = DetectPersonIdType(personIdOrEmail, personIdType);

            switch (personIdType)
            {
                case PersonIdType.Email:
                    teamMembership.PersonEmail = personIdOrEmail;
                    break;
                default:
                    teamMembership.PersonId = personIdOrEmail;
                    break;
            }

            teamMembership.IsModerator = isModerator;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<TeamMembership>, TeamMembership>(
                                    HttpMethod.Post,
                                    SPARK_TEAM_MEMBERSHIPS_API_URI,
                                    null,
                                    teamMembership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Create a team membership.
        /// </summary>
        /// <param name="team">The <see cref="Team"/>.</param>
        /// <param name="person">The <see cref="Person"/>.</param>
        /// <param name="isModerator">Set to true to make the person a room moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<TeamMembership> > CreateTeamMembershipAsync(Team team, Person person, bool? isModerator = null, CancellationToken? cancellationToken = null)
        {
            return (await CreateTeamMembershipAsync(team.Id, person.Id, isModerator, PersonIdType.Id, cancellationToken));
        }


        /// <summary>
        /// Gets team membership detail.
        /// </summary>
        /// <param name="membershipId">Team Membership id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<TeamMembership> > GetTeamMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<TeamMembership>, TeamMembership>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_TEAM_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets team membership detail.
        /// </summary>
        /// <param name="membeship"><see cref="TeamMembership"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<TeamMembership> > GetTeamMembershipAsync(TeamMembership membeship, CancellationToken? cancellationToken = null)
        {
            return (await GetTeamMembershipAsync(membeship.Id, cancellationToken));
        }


        /// <summary>
        /// Updates team membership.
        /// </summary>
        /// <param name="membershipId">Membership id to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a team moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<TeamMembership> > UpdateTeamMembershipAsync(string membershipId, bool isModerator, CancellationToken? cancellationToken = null)
        {
            var membership = new TeamMembership();

            membership.IsModerator = isModerator;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<TeamMembership>, TeamMembership>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", SPARK_TEAM_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    membership,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates team membership.
        /// </summary>
        /// <param name="membership"><see cref="TeamMembership"/> to be updated.</param>
        /// <param name="isModerator">Set to true to make the person a team moderator.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<TeamMembership> > UpdateTeamMembershipAsync(TeamMembership membership, bool isModerator, CancellationToken? cancellationToken = null)
        {
            return (await UpdateTeamMembershipAsync(membership.Id, isModerator, cancellationToken));
        }


        /// <summary>
        /// Deletes a team membership.
        /// </summary>
        /// <param name="membershipId">Team Membership id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteTeamMembershipAsync(string membershipId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", SPARK_TEAM_MEMBERSHIPS_API_PATH, Uri.EscapeDataString(membershipId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a team membership.
        /// </summary>
        /// <param name="membership"><see cref="TeamMembership"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteTeamMembershipAsync(TeamMembership membership, CancellationToken? cancellationToken = null)
        {
            return (await DeleteTeamMembershipAsync(membership.Id, cancellationToken));
        }

        #endregion


        #region Webhooks APIs

        /// <summary>
        /// Lists webhooks.
        /// </summary>
        /// <param name="max">Limit the maximum number of webhooks in the response.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkListResult<WebhookList> > ListWebhooksAsync(int? max = null, CancellationToken? cancellationToken = null)
        {
            var queryParameters = new NameValueCollection();

            queryParameters.Add("max", max?.ToString());

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkListResult<WebhookList>, WebhookList>(
                                    HttpMethod.Get,
                                    SPARK_WEBHOOKS_API_URI,
                                    queryParameters,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Create a webhook.
        /// </summary>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="resouce">The resource type for the webhook. Creating a webhook requires 'read' scope on the resource the webhook is for.</param>
        /// <param name="eventType">The event type for the webhook.</param>
        /// <param name="filters">The filter that defines the webhook scope.</param>
        /// <param name="secret">Secret used to generate payload signature.</param>
        /// <param name="secretLength">Secret length that is generated, if the secret parameter is null.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Webhook> > CreateWebhookAsync(string name, Uri targetUri, EventResource resouce, EventType eventType, IEnumerable<EventFilter> filters = null, string secret = null, int secretLength = 64, CancellationToken? cancellationToken = null)
        {
            var webhook = new Webhook();

            webhook.Name          = name;
            webhook.TargetUrl     = targetUri.AbsoluteUri;
            webhook.ResourceName  = resouce.Name;
            webhook.EventTypeName = eventType.Name;

            if(filters != null)
            {
                NameValueCollection nvc = new NameValueCollection();

                foreach (var item in filters)
                {
                    if((item is EventFilter.SpaceTypeFilter) && (resouce == EventResource.Space))
                    {
                        nvc.Add("type", item.Value);
                    }
                    else
                    {
                        nvc.Add(item.Key, item.Value);
                    }
                }

                webhook.Filter = HttpUtils.BuildQueryParameters(nvc);
            }

            if(secret != null)
            {
                webhook.Secret = secret;
            }
            else if(secretLength > 0)
            {
                webhook.Secret = new String(RAND.GetASCIIChars(secretLength, (CryptoRandom.ASCIICategory.UpperAlphabet | CryptoRandom.ASCIICategory.LowerAlphabet | CryptoRandom.ASCIICategory.Number)));
            }

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Webhook>, Webhook>(
                                    HttpMethod.Post,
                                    SPARK_WEBHOOKS_API_URI,
                                    null,
                                    webhook,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }


        /// <summary>
        /// Gets webhook detail.
        /// </summary>
        /// <param name="webhookId">Webhook id that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Webhook> > GetWebhookAsync(string webhookId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Webhook>, Webhook>(
                                    HttpMethod.Get,
                                    new Uri(String.Format("{0}/{1}", SPARK_WEBHOOKS_API_PATH, Uri.EscapeDataString(webhookId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets webhook detail.
        /// </summary>
        /// <param name="webhook"><see cref="Webhook"/> that the detail info is gotten.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Webhook> > GetWebhookAsync(Webhook webhook, CancellationToken? cancellationToken = null)
        {
            return (await GetWebhookAsync(webhook.Id, cancellationToken));
        }


        /// <summary>
        /// Updates webhook.
        /// </summary>
        /// <param name="webhookId">Webhook id to be updated.</param>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Webhook> > UpdateWebhookAsync(string webhookId, string name, Uri targetUri, CancellationToken? cancellationToken = null)
        {
            var webhook = new Webhook();

            webhook.Name      = name;
            webhook.TargetUrl = targetUri.AbsoluteUri;

            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<Webhook>, Webhook>(
                                    HttpMethod.Put,
                                    new Uri(String.Format("{0}/{1}", SPARK_WEBHOOKS_API_PATH, Uri.EscapeDataString(webhookId))),
                                    null,
                                    webhook,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Updates webhook.
        /// </summary>
        /// <param name="webhook"><see cref="Webhook"/> to be updated.</param>
        /// <param name="name">A user-friendly name for this webhook.</param>
        /// <param name="targetUri">The URL that receives POST requests for each event.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<Webhook> > UpdateWebhookAsync(Webhook webhook, string name, Uri targetUri, CancellationToken? cancellationToken = null)
        {
            return (await UpdateWebhookAsync(webhook.Id, name, targetUri, cancellationToken));
        }


        /// <summary>
        /// Deletes a webhook.
        /// </summary>
        /// <param name="webhookId">Webhook id to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteWebhookAsync(string webhookId, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestJsonAsync<SparkResult<NoContent>, NoContent>(
                                    HttpMethod.Delete,
                                    new Uri(String.Format("{0}/{1}", SPARK_WEBHOOKS_API_PATH, Uri.EscapeDataString(webhookId))),
                                    null,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.NoContent));

            return result;
        }

        /// <summary>
        /// Deletes a webhook.
        /// </summary>
        /// <param name="webhook"><see cref="Webhook"/> to be deleted.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<NoContent> > DeleteWebhookAsync(Webhook webhook, CancellationToken? cancellationToken = null)
        {
            return (await DeleteWebhookAsync(webhook.Id, cancellationToken));
        }

        #endregion


        #region Files APIs

        /// <summary>
        /// Gets file info.
        /// </summary>
        /// <param name="fileUri">Uri of the file.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SparkFileInfo> > GetFileInfoAsync(Uri fileUri, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestFileInfoAsync<SparkResult<SparkFileInfo>, SparkFileInfo>(
                                    HttpMethod.Head,
                                    fileUri,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        /// <summary>
        /// Gets file data.
        /// </summary>
        /// <param name="fileUri">Uri of the file.</param>
        /// <param name="cancellationToken"><see cref="CancellationToken"/> to be used for cancellation.</param>
        /// <returns><see cref="SparkResult{TSparkObject}"/> to get result.</returns>
        public async Task< SparkResult<SparkFileData> > GetFileDataAsync(Uri fileUri, CancellationToken? cancellationToken = null)
        {
            var result = await this.sparkHttpClient.RequestFileInfoAsync<SparkResult<SparkFileData>, SparkFileData>(
                                    HttpMethod.Get,
                                    fileUri,
                                    null,
                                    cancellationToken);

            result.IsSuccessStatus = (result.IsSuccessStatus && (result.HttpStatusCode == System.Net.HttpStatusCode.OK));

            return result;
        }

        #endregion




        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <summary>
        /// Dispose.
        /// </summary>
        /// <param name="disposing">disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    using (this.sparkHttpClient)
                    {
                        // disposed.
                    }
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~SparkAPIClient() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        /// <summary>
        /// Dispose.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

    }

}
