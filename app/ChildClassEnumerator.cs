using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

/*
 * Retourne une liste d'objets contenant toutes les classes filles de <T>
 * Utilisé pour instancier tous les bots
 */

public static class ChildClassEnumerator {
    static ChildClassEnumerator() { }

    public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs)where T : class {
        List<T> objects = new List<T>();

        var types = Assembly.GetAssembly(typeof(T)).GetTypes().Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(T)));

        foreach (Type type in types) {
            objects.Add((T)Activator.CreateInstance(type, constructorArgs));
        }
        return objects;
    }
}