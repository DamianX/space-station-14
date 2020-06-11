using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Content.Server.GameObjects.Components.Security;
using Robust.Shared.GameObjects;
using Robust.Shared.GameObjects.Systems;
using Robust.Shared.Utility;
using static Content.Shared.GameObjects.Components.Security.SharedSecurityCameraConsoleComponent;

namespace Content.Server.GameObjects.EntitySystems
{
    public class SecurityCameraConsoleEntitySystem : EntitySystem
    {
        public Dictionary<string, List<SecurityCameraEntry>> Cameras = new Dictionary<string, List<SecurityCameraEntry>>();

        public override void Initialize()
        {
            base.Initialize();
            EntityQuery = new TypeEntityQuery(typeof(SecurityCameraConsoleComponent));
        }

        public void ChangeCameraCategory(Guid guid, string newCategory)
        {
            foreach (var (categoryName, category) in Cameras)
            {
                foreach (var entry in category.Where(entry => entry.Identifier == guid))
                {
                    Debug.Assert(categoryName != newCategory);
                    category.Remove(entry);

                    var existingCategory = Cameras.FirstOrDefault(pair => pair.Key == newCategory);
                    if (existingCategory.IsNull())
                    {
                        Cameras.Add(newCategory, new List<SecurityCameraEntry>{entry});
                    }
                    else
                    {
                        existingCategory.Value.Add(entry);
                    }
                    UpdateAllConsoles();
                    break;
                }
            }
            throw new ArgumentException($"Expected GUID {guid} to be present in one of the categories but it was not.");
        }

        public void ChangeCameraTag(Guid guid, string newTag)
        {
            foreach (var category in Cameras.Values)
            {
                foreach (var entry in category.Where(entry => entry.Identifier == guid))
                {
                    Debug.Assert(entry.Name != newTag);

                    entry.Name = newTag;
                    UpdateAllConsoles();
                    break;
                }
            }
            throw new ArgumentException($"Expected GUID {guid} to be present in one of the categories but it was not.");
        }

        public void AddCamera(Guid guid, string category, string tag)
        {
            var existingPair = Cameras.FirstOrDefault(pair => pair.Key == category);
            if (existingPair.IsNull())
            {
                Cameras.Add(category, new List<SecurityCameraEntry> {new SecurityCameraEntry(tag, guid)});
            }
            else
            {
                existingPair.Value.Add(new SecurityCameraEntry(tag, guid));
            }

            UpdateAllConsoles();
        }

        public void RemoveCamera(Guid guid)
        {
            foreach (var category in Cameras.Values)
            {
                category.RemoveAll(entry => entry.Identifier == guid);
            }

            UpdateAllConsoles();
        }

        private void UpdateAllConsoles()
        {
            foreach (var entity in RelevantEntities)
            {
                var component = entity.GetComponent<SecurityCameraConsoleComponent>();
                component.UpdateState(Cameras);
            }
        }
    }
}
