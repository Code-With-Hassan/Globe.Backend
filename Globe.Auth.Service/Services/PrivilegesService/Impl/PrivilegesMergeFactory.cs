using Globe.Shared.Models.Privileges;

namespace Globe.Account.Service.Services.PrivilegesService.Impl
{
    /// <summary>
    /// Multiple Similar privileges merge factory.
    /// This factory class will merge privileges and returns distinct privileges based on the best available
    /// privilege setting.
    /// </summary>
    internal class PrivilegesMergeFactory
    {
        readonly List<RoleScreenModel> _screenList = new();

        /// <summary>
        /// Adds the role screen model to list
        /// </summary>
        /// <param name="model">The model.</param>
        internal void Add(RoleScreenModel model)
        {
            _screenList.Add(model);
        }

        /// <summary>
        /// Gets the merged role screen model list.
        /// </summary>
        /// <returns>A list of RoleScreenModelS.</returns>
        internal List<RoleScreenModel> GetMerged()
        {
            var list = new List<RoleScreenModel>();
            for (int i = 0; i < _screenList.Count; i++)
            {
                long screenId = _screenList[i].ScreenId;
                list.Add(MergeIdentical(_screenList.FindAll(x => x.ScreenId == screenId)));

                _screenList.RemoveAll(x => x.ScreenId == screenId);
                i = -1;
            }
            return list;
        }

        /// <summary>
        /// Merges the identical role screen models.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>A RoleScreenModel.</returns>
        private static RoleScreenModel MergeIdentical(List<RoleScreenModel> list)
        {
            // return if only 1 model in list.
            if (list.Count == 1) return list[0];

            // Hold 1st screen to start with.
            var model = list[0];

            // Merge Screen Privileges.
            for (int i = 1; i < list.Count; i++)
            {
                model.ScreenPrivileges = GetBestScreenPrivilege(model.ScreenPrivileges, list[i].ScreenPrivileges);
            }

            return model;
        }

        /// <summary>
        /// Gets the best screen privilege.
        /// </summary>
        /// <param name="p1">The first privilege value.</param>
        /// <param name="p2">The second privilege value.</param>
        /// <returns>privilege as int.</returns>
        private static int GetBestScreenPrivilege(int p1, int p2)
        {
            if (p1 == 0) return p2;
            else if (p1 == 1) return p1;
            else if (p1 == 2) return p2 == 1 ? p2 : p1;
            else //if(p1 == 3) 
                return p2 == 0 ? p1 : p2;

        }
    }
}