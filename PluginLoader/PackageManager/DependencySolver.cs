using Semver;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PluginLoader
{
    public static class DependencySolver
    {
        public static UnmetDependency[] FirstLevelUnmetDependencies(UnmetDependency[] unmetDependencies)
        {
            var newUnmetDependencies = unmetDependencies.ToList();
            // If package listed as dependency in another package doesn't meet that dependency and at the same time
            // this package has some unmet dependencies, these unmet dependencies are removed from list
            // Basically cuts C from A -> B -> C chain, if B doesn't meet requirements of A and C doesn't meet requirements of B
            newUnmetDependencies.RemoveAll(thisUD =>
                unmetDependencies.Any(secondUD =>
                    thisUD.Package.Metadata.UniqueName == secondUD.Dependency.UniqueName));
            return newUnmetDependencies.ToArray();
        }

        public static bool Solve(Package[] packages, out UnmetDependency[] unmetDependencies)
        {
            var unmetDependenciesList = new List<UnmetDependency>();
            // First we consider every dependency as unmet
            packages.ToList().ForEach(package =>
            {
                // PluginLoader dependency
                unmetDependenciesList.Add(new UnmetDependency
                {
                    Package = package,
                    Dependency = new Dependency
                    {
                        UniqueName = nameof(PluginLoader),
                        Version = package.Metadata.PLVersion
                    }
                });

                // Dependencies
                unmetDependenciesList.AddRange(
                    package.Metadata.Dependencies.ConvertAll(dependency =>
                    new UnmetDependency
                    {
                        Package = package,
                        Dependency = dependency
                    }));
            });

            // Now we get rid of unmet dependencies which can be solved
            unmetDependenciesList.ToList().ForEach(unmetDependency =>
            {
                if (unmetDependency.Dependency.UniqueName == nameof(PluginLoader) &&
                    versionsAreCompatible(unmetDependency.Dependency.Version, Versions.PLVersion))
                    unmetDependenciesList.Remove(unmetDependency);
                else if (packages.Any(package => meetsDependency(package, unmetDependency.Dependency)))
                    unmetDependenciesList.Remove(unmetDependency);
            });

            unmetDependencies = unmetDependenciesList.ToArray();
            return unmetDependencies.Length == 0;
        }
        public static Package[] Sort(Package[] packages)
        {
            // What about deeply looped dependencies?
            // Recursion? Is it worth implementation? Do we need it?
            Array.Sort(packages, delegate (Package package1, Package package2)
            {
                var p1p2 = dependsOn(package1, package2);
                var p2p1 = dependsOn(package2, package1);

                if (p1p2 && p2p1)
                    throw new DependencyLoopException(package1, package2);

                return p1p2 ? 1 : p2p1 ? -1 : 0;
            });
            return packages;
        }

        private static bool dependsOn(Package package1, Package package2)
        {
            return package1.Metadata.Dependencies.Any(dependency =>
                dependency.UniqueName == package2.Metadata.UniqueName);
        }
        private static bool meetsDependency(Package package, Dependency dependency)
        {
            return package.Metadata.UniqueName == dependency.UniqueName &&
                versionsAreCompatible(package.Metadata.Version, dependency.Version);
        }

        private static bool versionsAreCompatible(SemVersion version1, SemVersion version2)
        {
            return (version1.Major == version2.Major) &&
                ((version1.Major == 0 && version1.Minor == version2.Minor) || version1.Major != 0);
        }
    }

    public class DependencyLoopException : Exception
    {
        public DependencyLoopException(Package package1, Package package2) :
            base($"Loop dependency between {package1.ToString()} and {package2.ToString()}") { }
    }
}
