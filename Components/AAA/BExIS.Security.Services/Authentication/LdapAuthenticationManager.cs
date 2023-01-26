using BExIS.Security.Entities.Authentication;
using BExIS.Security.Entities.Subjects;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.DirectoryServices.Protocols;
using System.Globalization;
using System.Net;
using Vaiona.Persistence.Api;

namespace BExIS.Security.Services.Authentication
{
    public class LdapAuthenticationManager
    {
        private readonly IUnitOfWork _guow;
        private bool _isDisposed;

        private readonly LdapConfiguration _ldapConfiguration;

        public LdapAuthenticationManager()
        {
            _guow = this.GetIsolatedUnitOfWork();
            _ldapConfiguration = new LdapConfiguration();
        }

        ~LdapAuthenticationManager()
        {
            Dispose(true);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public User GetUser(string username, string password)
        {
            User ldapUser = null;
            try
            {
                using (var ldap = new LdapConnection(new LdapDirectoryIdentifier(_ldapConfiguration.HostName, _ldapConfiguration.HostPort)))
                {
                    ldap.SessionOptions.ProtocolVersion = _ldapConfiguration.HostVersion;
                    ldap.AuthType = (AuthType)_ldapConfiguration.HostAuthType;
                    ldap.SessionOptions.SecureSocketLayer = _ldapConfiguration.HostSsl;
                    ldap.Credential = new NetworkCredential($"{_ldapConfiguration.UserIdentifier}={username},{_ldapConfiguration.HostBaseDn}", password);
                    ldap.Bind();

                    var searchResponse = (SearchResponse)ldap.SendRequest(new SearchRequest($"{_ldapConfiguration.UserIdentifier}={username},{_ldapConfiguration.HostBaseDn}", "objectClass=*", (SearchScope)_ldapConfiguration.HostScope));

                    if (searchResponse.Entries.Count == 1)
                    {
                        var attributes = searchResponse.Entries[0].Attributes;

                        ldapUser = new User()
                        {
                            Email = (attributes["mail"][0]).ToString(),
                            UserName = (attributes[$"{_ldapConfiguration.UserIdentifier}"][0]).ToString(),
                            IsEmailConfirmed = true,
                            //HasPrivacyPolicyAccepted = true,
                            //HasTermsAndConditionsAccepted = true
                        };
                    }
                    else
                    {
                        throw new Exception("User not found");
                    }
                }
            }
            catch (Exception e)
            {
                //Todo: Pass error to logging framework instead of console!
                ldapUser = null;
            }
            
            return ldapUser;
        }

        public SignInStatus ValidateUser(string username, string password)
        {
            var ldapuser = GetUser(username, password);

            if (ldapuser != null)
                return SignInStatus.Success;
            return SignInStatus.Failure;
        }

        public Tuple<User, string> GetUser2(string username, string password)
        {
            User ldapUser = null;
            string errors = "";
            try
            {
                using (var ldap = new LdapConnection(new LdapDirectoryIdentifier(_ldapConfiguration.HostName, _ldapConfiguration.HostPort)))
                {
                    ldap.SessionOptions.ProtocolVersion = _ldapConfiguration.HostVersion;
                    ldap.AuthType = (AuthType)_ldapConfiguration.HostAuthType;
                    ldap.SessionOptions.SecureSocketLayer = _ldapConfiguration.HostSsl;
                    //ldap.Credential = new NetworkCredential($"{_ldapConfiguration.UserIdentifier}={username},{_ldapConfiguration.HostBaseDn}", password);
                    ldap.Bind();

                    SearchRequest searchRequest = new SearchRequest(
                        $"{_ldapConfiguration.HostBaseDn}",
                        string.Format(CultureInfo.InvariantCulture, "{0}={1}", $"{_ldapConfiguration.UserIdentifier}", username),
                        SearchScope.Subtree
                    );
                    
                    var searchResponse = (SearchResponse)ldap.SendRequest(searchRequest);
                    if (1 == searchResponse.Entries.Count)
                    {
                        ldap.Bind(new NetworkCredential(searchResponse.Entries[0].DistinguishedName, password));

                        var attributes = searchResponse.Entries[0].Attributes;

                        ldapUser = new User()
                        {
                            Email = (attributes["mail"]?[0] ?? " ").ToString(),
                            UserName = (attributes["cn"]?[0] ?? " ").ToString(),
                            IsEmailConfirmed = true
                        };
                    }
                    else
                    {
                        throw new Exception("User not found");
                    }

                    /*
                    var searchResponse = (SearchResponse)ldap.SendRequest(
                        new SearchRequest($"{_ldapConfiguration.UserIdentifier}={username}," +
                        $"{_ldapConfiguration.HostBaseDn}", 
                        "objectClass=*", 
                        (SearchScope)_ldapConfiguration.HostScope));
                    
                    if (searchResponse.Entries.Count == 1)
                    {
                        var attributes = searchResponse.Entries[0].Attributes;

                        ldapUser = new User()
                        {
                            Email = (attributes["mail"][0]).ToString(),
                            UserName = (attributes[$"{_ldapConfiguration.UserIdentifier}"][0]).ToString(),
                            IsEmailConfirmed = true,
                            //HasPrivacyPolicyAccepted = true,
                            //HasTermsAndConditionsAccepted = true
                        };
                    }
                    else
                    {
                        throw new Exception("User not found");
                    }
                    */
                }
            }
            catch (Exception e)
            {
                //Todo: Pass error to logging framework instead of console!
                ldapUser = null;
                errors = e.Message;
            }
            Tuple <User, string> dict = new Tuple <User, string>(ldapUser, errors); 
            return dict;
        }
        public Tuple<SignInStatus,string> ValidateUser2(string username, string password)
        {
            Tuple<User, string> dict = GetUser2(username, password);
            var ldapuser = dict.Item1;
            if (ldapuser != null)
                return new Tuple<SignInStatus, string > (SignInStatus.Success, "");
            ;
            return new Tuple<SignInStatus,string> (SignInStatus.Failure,dict.Item2);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    if (_guow != null)
                        _guow.Dispose();
                    _isDisposed = true;
                }
            }
        }
    }
}