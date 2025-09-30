using UnityEngine;


public static class OptionSelectionService
{

    /// Activates only the child with the given name and disables all others.
    /// <param name="parent">The parent GameObject whose children will be toggled.</param>
    /// <param name="childName">The name of the child to enable.</param>
    public static void SelectOption(GameObject parent, string childName)
    {
        if (parent == null || string.IsNullOrEmpty(childName)) return;

        int childCount = parent.transform.childCount;
        Transform targetChild = null;

        for (int i = 0; i < childCount; i++)
        {
            Transform child = parent.transform.GetChild(i);
            child.gameObject.SetActive(false);

            if (child.name.Equals(childName, System.StringComparison.Ordinal))
            {
                targetChild = child;
            }
        }

        if (targetChild != null)
        {
            targetChild.gameObject.SetActive(true);
        }
    }
}

