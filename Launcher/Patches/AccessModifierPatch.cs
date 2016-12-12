using System.Linq;

namespace Launcher.Patches
{
    internal class AccessModifierPatch : Patch
    {
        public AccessModifierPatch(PatcherData patcherData) : base(patcherData)
        {
            allTypesPublic();
            allNotPublicMethodsProtected();
            allPrivateFieldsProtected();
        }

        private void allNotPublicMethodsProtected()
        {
            patcherData.IRData.Module.GetTypes().ToList().ForEach(type =>
                type.Methods.ToList().ForEach(method => {
                if ((method.IsPrivate || method.IsAssembly || method.IsFamilyAndAssembly))
                    method.IsFamily = true;
            }));
        }

        private void allPrivateFieldsProtected()
        {
            patcherData.IRData.Module.GetTypes().ToList().ForEach(type =>
                type.Fields.ToList().ForEach(field => {
                if (field.IsPrivate)
                    field.IsFamily = true;
            }));
        }

        private void allTypesPublic()
        {
            patcherData.IRData.Module.GetTypes().ToList().ForEach(type => {
                if (!type.IsNested)
                    type.IsPublic = true;
                else
                    type.IsNestedPublic = true;
            });
        }
    }
}