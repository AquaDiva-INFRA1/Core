using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BExIS.Security.Entities.Subjects;
using Vaiona.Persistence.Api;

namespace BExIS.Security.Services.Subjects
{
    public class UserPiManager
    {
        public IReadOnlyRepository<UserPi> UserPisRepo { get; private set; }
        public IReadOnlyRepository<User> UsersRepo { get; private set; }

        public UserPiManager()
        {
            IUnitOfWork uow = this.GetUnitOfWork();

            this.UserPisRepo = uow.GetReadOnlyRepository<UserPi>();
            this.UsersRepo = uow.GetReadOnlyRepository<User>();
        }

        public UserPi GetUserPiById(long id)
        {
            UserPi userPi = UserPisRepo.Get(upis => upis.Id == id).FirstOrDefault();

            if (userPi != null)
            {
                return userPi;
            }

            return null;
        }

        public IQueryable<User> GetPisFromUserById(long id)
        {
            List<long> piIds = UserPisRepo.Query(u => u.UserId == id).Select(u => u.PiId).ToList<long>();

            return UsersRepo.Query(u => piIds.Contains(u.Id));
        }


        public IQueryable<User> GetPisFromUserByName(string userName)
        {
            User user = UsersRepo.Get(u => u.Name.ToLower() == userName.ToLower()).FirstOrDefault();

            if (user != null)
            {
                List<long> piIds = UserPisRepo.Query(u => u.UserId == user.Id).Select(u => u.PiId).ToList<long>();

                return UsersRepo.Query(u => piIds.Contains(u.Id));
            }

            return null;
        }


        public UserPi GetUserPi(long UserId, long PiId)
        {
            ICollection<UserPi> userPis = UserPisRepo.Query(u => u.UserId == UserId && u.PiId == PiId).ToArray();

            if (userPis.Count() == 1)
            {
                return userPis.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public UserPi GetUserPi(long id)
        {
            ICollection<UserPi> userPis = UserPisRepo.Query(u => u.Id == id).ToArray();

            if (userPis.Count() == 1)
            {
                return userPis.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }


        public UserPi AddUserPi(long UserId, long PiId)
        {
            UserPi newUserPi = null;
            UserPi searchUserPi = GetUserPi(UserId, PiId);

            if (searchUserPi == null)
            {
                newUserPi = new UserPi(UserId, PiId);
                if (newUserPi != null)
                {
                    using (IUnitOfWork uow = this.GetUnitOfWork())
                    {
                        IRepository<UserPi> userPiRepo = uow.GetRepository<UserPi>();
                        userPiRepo.Put(newUserPi);
                        uow.Commit();
                    }
                    return newUserPi;
                }
                else
                {
                    throw new NullReferenceException("UserPi was not created");
                }
            }
            else
            {
                throw new ArgumentException("The UserPi with UserId [" + UserId + "] and PiId [" + PiId + "] already exist");
            }
        }

        public void DeleteUserPi(long UserId, long PiId)
        {
            UserPi userPi = GetUserPi(UserId, PiId);

            if (userPi != null)
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<UserPi> userPiRepo = uow.GetRepository<UserPi>();

                    userPiRepo.Delete(userPi);
                    uow.Commit();
                }
            }
            else
            {
                throw new NullReferenceException("UserPi cannot be found");
            }
        }

        public bool IsUserWithPi(long UserId, long PiId)
        {
            if (GetUserPi(UserId, PiId) == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public ICollection<UserPi> GetAllUserPis()
        {
            ICollection<UserPi> userPis = UserPisRepo.Query(u => u.Id != 0).ToArray();

            if (userPis.Count() > 0)
            {
                return userPis;
            }
            else
            {
                return null;
            }
        }

        public UserPi EditUserPi(UserPi userPi)
        {
            using (IUnitOfWork uow = this.GetUnitOfWork())
            {
                IRepository<UserPi> userPiRepo = uow.GetRepository<UserPi>();
                userPiRepo.Put(userPi);
                uow.Commit();
            }

            return (userPi);
        }

        public void DeleteUserPi(long id)
        {
            UserPi userPi = GetUserPi(id);

            if (userPi != null)
            {
                using (IUnitOfWork uow = this.GetUnitOfWork())
                {
                    IRepository<UserPi> userPiRepo = uow.GetRepository<UserPi>();

                    userPiRepo.Delete(userPi);
                    uow.Commit();
                }
            }
            else
            {
                throw new NullReferenceException("UserPi cannot be found");
            }
        }

        public ICollection<UserPi> GetAllPiMember(long piId)
        {
            ICollection<UserPi> userPis = UserPisRepo.Query(u => u.PiId == piId).ToArray();

            if (userPis.Count() > 0)
            {
                return userPis;
            }
            else
            {
                return null;
            }
        }
    }
}
