//$ Copyright 2016, Code Respawn Technologies Pvt Ltd - All Rights Reserved $//

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DungeonArchitect.Utils;
using DungeonArchitect.Graphs;
using DungeonArchitect.SpatialConstraints;

namespace DungeonArchitect
{
	using PropBySocketType_t = Dictionary<string, List<PropTypeData>>;
	using PropBySocketTypeByTheme_t = Dictionary<DungeonPropDataAsset, Dictionary<string, List<PropTypeData>>>;


    /// <summary>
    /// Builds the layout of the dungeon and emits markers around the layout
    /// Implement this class to create your own builder
    /// </summary>
	[ExecuteInEditMode]
    public abstract class DungeonBuilder : MonoBehaviour
    {
        protected DungeonConfig config;
        protected PMRandom nrandom;
        protected PMRandom random;
        protected DungeonModel model;
        protected LevelMarkerList propSockets = new LevelMarkerList();
        protected int _SocketIdCounter = 0;
        protected Blackboard blackboard = new Blackboard();
        
        private bool isLayoutBuilt = false;
        public bool IsLayoutBuilt
        {
            get
            {
                {
                    // Hot code reload sometimes invalidates the model.  
                    if (random == null) return false;
                }
                return isLayoutBuilt;
            }
        }

        public LevelMarkerList PropSockets
        {
            get { return propSockets; }
        }

        public DungeonArchitect.DungeonModel Model
        {
            get { return model; }
        }

        public Blackboard Blackboard
        {
            get { return blackboard; }
        }

        /// <summary>
        /// Builds the dungeon layout
        /// </summary>
        /// <param name="config">The builder configuration</param>
        /// <param name="model">The dungeon model that the builder will populate</param>
        public virtual void BuildDungeon(DungeonConfig config, DungeonModel model)
        {
            this.config = config;
            this.model = model;

            nrandom = new PMRandom(config.Seed);
            random = new PMRandom(config.Seed);

            propSockets = CreateMarkerListObject(config);

            isLayoutBuilt = true;
        }

        protected virtual LevelMarkerList CreateMarkerListObject(DungeonConfig config)
        {
            return new SpatialPartionedLevelMarkerList(8);
        }

		public virtual void OnDestroyed() {
			ClearSockets();
			isLayoutBuilt = false;
		}


        public virtual bool IsThemingSupported() { return true; }

        // This is called by the builders that do not support theming
        public virtual void BuildNonThemedDungeon(DungeonSceneProvider sceneProvider, IDungeonSceneObjectInstantiator objectInstantiator) { }
        
        public virtual void DebugDraw()
        {
        }

        public virtual void DebugDrawGizmos()
        {
        }

        /// <summary>
        /// Emit markers defined by this builder
        /// </summary>
        public virtual void EmitMarkers() { 
			ClearSockets();
		}

        /// <summary>
        /// Emit markers defined by the user (implementation of DungeonMarkerEmitter)
        /// </summary>
        public void EmitCustomMarkers()
        {
            var emitters = GetComponents<DungeonMarkerEmitter>();
            foreach (var emitter in emitters)
            {
                emitter.EmitMarkers(this);
            }
        }
		
		protected void ClearSockets()
		{
			_SocketIdCounter = 0;
			PropSockets.Clear();
		}

        public PropSocket EmitMarker(string SocketType, Matrix4x4 transform, IntVector gridPosition, int cellId)
        {
            PropSocket socket = new PropSocket();
            socket.Id = ++_SocketIdCounter;
            socket.IsConsumed = false;
            socket.SocketType = SocketType;
            socket.Transform = transform;
            socket.gridPosition = gridPosition;
            socket.cellId = cellId;
            PropSockets.Add(socket);
            return socket;
        }

        protected void EmitMarker(string SocketType, Matrix4x4 _transform, int count, Vector3 InterOffset, IntVector gridPosition, int cellId, Vector3 LogicalToWorldScale)
        {
            var iposition = new IntVector(gridPosition.x, gridPosition.y, gridPosition.z);
            var ioffset = new IntVector(
                Mathf.RoundToInt(InterOffset.x / LogicalToWorldScale.x),
                Mathf.RoundToInt(InterOffset.y / LogicalToWorldScale.y),
                Mathf.RoundToInt(InterOffset.z / LogicalToWorldScale.z)
            );
            Matrix4x4 transform = Matrix.Copy(_transform);
            var position = Matrix.GetTranslation(ref transform);

            for (int i = 0; i < count; i++)
            {
                EmitMarker(SocketType, transform, iposition, cellId);
                position += InterOffset;
                iposition += ioffset;
                transform = Matrix.Copy(transform);
                Matrix.SetTranslation(ref transform, position);
            }
        }

        protected void EmitMarker(List<PropSocket> pPropSockets, string SocketType, Matrix4x4 transform, IntVector gridPosition, int cellId)
        {
            PropSocket socket = new PropSocket();
            socket.Id = ++_SocketIdCounter;
            socket.IsConsumed = false;
            socket.SocketType = SocketType;
            socket.Transform = transform;
            socket.gridPosition = gridPosition;
            socket.cellId = cellId;
            pPropSockets.Add(socket);
        }


        protected void CreatePropLookup(DungeonPropDataAsset PropAsset, PropBySocketTypeByTheme_t PropBySocketTypeByTheme)
        {
            if (PropAsset == null || PropBySocketTypeByTheme.ContainsKey(PropAsset))
            {
                // Lookup for this theme has already been built
                return;
            }

            PropBySocketType_t PropBySocketType = new PropBySocketType_t();
            PropBySocketTypeByTheme.Add(PropAsset, PropBySocketType);

            foreach (PropTypeData Prop in PropAsset.Props)
            {
                if (!PropBySocketType.ContainsKey(Prop.AttachToSocket))
                {
                    PropBySocketType.Add(Prop.AttachToSocket, new List<PropTypeData>());
                }
                PropBySocketType[Prop.AttachToSocket].Add(Prop);
            }
        }

        /// <summary>
        /// Implementations should override this so that the new logical scale and position is set based on the volume's transformation
        /// </summary>
        /// <param name="volume"></param>
        /// <param name="newPositionOnGrid"></param>
        /// <param name="newSizeOnGrid"></param>
        public virtual void OnVolumePositionModified(Volume volume, out IntVector newPositionOnGrid, out IntVector newSizeOnGrid)
        {
            newPositionOnGrid = MathUtils.ToIntVector(volume.transform.position);
            newSizeOnGrid = MathUtils.ToIntVector(volume.transform.localScale);
        }

        // Picks a theme from the list that has a definition for the defined socket
        protected DungeonPropDataAsset GetBestMatchedTheme(List<DungeonPropDataAsset> Themes, PropSocket socket, PropBySocketTypeByTheme_t PropBySocketTypeByTheme)
        {
            var ValidThemes = new List<DungeonPropDataAsset>();
            foreach (DungeonPropDataAsset Theme in Themes)
            {
                if (PropBySocketTypeByTheme.ContainsKey(Theme))
                {
                    PropBySocketType_t PropBySocketType = PropBySocketTypeByTheme[Theme];
                    if (PropBySocketType.Count > 0)
                    {
                        if (PropBySocketType.ContainsKey(socket.SocketType) && PropBySocketType[socket.SocketType].Count > 0)
                        {
                            ValidThemes.Add(Theme);
                        }
                    }
                }
            }
            if (ValidThemes.Count == 0)
            {
                return null;
            }

            int index = Mathf.FloorToInt(random.GetNextUniformFloat() * ValidThemes.Count) % ValidThemes.Count;
            return ValidThemes[index];
        }

        protected virtual bool ProcessSpatialConstraint(SpatialConstraintProcessor constraintProcessor, SpatialConstraintAsset constraint, PropSocket socket, out Matrix4x4 OutOffset, out PropSocket[] outMarkersToRemove)
        {
            if (constraintProcessor == null)
            {
                OutOffset = Matrix4x4.identity;
                outMarkersToRemove = new PropSocket[0];
                return false;
            }
            var context = new SpatialConstraintProcessorContext();
            context.constraintAsset = constraint;
            context.marker = socket;
            context.model = model;
            context.config = config;
            context.builder = this;
            context.levelMarkers = propSockets;
            return constraintProcessor.ProcessSpatialConstraint(context, out OutOffset, out outMarkersToRemove);
        }

        protected void ProcessMarkerOverrideVolumes()
        {
			var dungeon = GetComponent<Dungeon>();
            // Process the theme override volumes
            var replacementVolumes = GameObject.FindObjectsOfType<MarkerReplaceVolume>();
            foreach (var replacementVolume in replacementVolumes)
            {
				if (replacementVolume.dungeon == dungeon) {
					// Fill up the prop sockets with the defined mesh data 
					for (int i = 0; i < propSockets.Count; i++) {
						PropSocket socket = propSockets[i];
						var socketPosition = Matrix.GetTranslation (ref socket.Transform);
						if (replacementVolume.GetBounds ().Contains (socketPosition)) {
							foreach (var replacementEntry in replacementVolume.replacements) {
								if (socket.SocketType == replacementEntry.fromMarker) {
									socket.SocketType = replacementEntry.toMarker;
								}
							}
						}
					}
				}
            }
        }

        void TagDungeonItemUserData(GameObject dungeonItem, int cellID)
        {
            if (dungeonItem == null) return;

            var data = dungeonItem.GetComponent<DungeonSceneProviderData>();
            if (data != null)
            {
                data.userData = cellID;
            }
        }

        // The data for executing all the nodes attached under a marker
        struct NodeListExecutionData
        {
            public List<PropTypeData> nodeDataList;
            public PropSocket socket;
        }

        // The context required for executing all the nodes attached under a marker
        struct NodeListExecutionContext
        {
            public InstanceCache instanceCache;
            public SpatialConstraintProcessor constraintProcessor;
            public PMRandom srandom;
            public DungeonSceneProvider SceneProvider;
            public IDungeonSceneObjectInstantiator objectInstantiator;
        };

        struct NodeSpawnData
        {
            public PropTypeData nodeData;
            public Matrix4x4 transform;
            public PropSocket socket;
        }
        
        public virtual void ApplyTheme(List<DungeonPropDataAsset> Themes, DungeonSceneProvider SceneProvider, 
                IDungeonSceneObjectInstantiator objectInstantiator)
        {
			var instanceCache = new InstanceCache();
            var constraintProcessor = GetComponent<SpatialConstraintProcessor>();

            PropBySocketTypeByTheme_t PropBySocketTypeByTheme = new PropBySocketTypeByTheme_t();
            foreach (DungeonPropDataAsset Theme in Themes)
            {
                CreatePropLookup(Theme, PropBySocketTypeByTheme);
            }

            // Collect all the theme override volumes and prepare their theme lookup
            var overrideVolumes = new List<ThemeOverrideVolume>();
            Dictionary<Graph, DungeonPropDataAsset> GraphToThemeMapping = new Dictionary<Graph, DungeonPropDataAsset>();

			var dungeon = GetComponent<Dungeon>();

            // Process the theme override volumes
            var themeOverrides = GameObject.FindObjectsOfType<ThemeOverrideVolume>();
            foreach (var volume in themeOverrides)
            {
				if (volume.dungeon != dungeon) {
					continue;
				}
				
                overrideVolumes.Add(volume);
                var graph = volume.overrideTheme;
                if (graph != null && !GraphToThemeMapping.ContainsKey(graph))
                {
                    DungeonPropDataAsset theme = new DungeonPropDataAsset();
                    theme.BuildFromGraph(volume.overrideTheme);
                    GraphToThemeMapping.Add(volume.overrideTheme, theme);

                    CreatePropLookup(theme, PropBySocketTypeByTheme);
                }
            }
            
            var srandom = new PMRandom(config.Seed);

            var nodesExecutionContext = new NodeListExecutionContext();
            nodesExecutionContext.instanceCache = instanceCache;
            nodesExecutionContext.constraintProcessor = constraintProcessor;
            nodesExecutionContext.srandom = srandom;
            nodesExecutionContext.SceneProvider = SceneProvider;
            nodesExecutionContext.objectInstantiator = objectInstantiator;
            
            var spawnDataList = new List<NodeSpawnData>();

            var delayedExecutionList = new Queue<NodeListExecutionData>();
            // Fill up the prop sockets with the defined mesh data 
            for (int i = 0; i < PropSockets.Count; i++)
            {
                PropSocket socket = PropSockets[i];
                if (!socket.markForDeletion)
                {
                    DungeonPropDataAsset themeToUse = GetBestMatchedTheme(Themes, socket, PropBySocketTypeByTheme); // PropAsset;

                    // Check if this socket resides within a override volume
                    {
                        var socketPosition = Matrix.GetTranslation(ref socket.Transform);
                        foreach (var volume in overrideVolumes)
                        {
                            if (volume.GetBounds().Contains(socketPosition))
                            {
                                var graph = volume.overrideTheme;
                                if (graph != null && GraphToThemeMapping.ContainsKey(graph))
                                {
                                    themeToUse = GraphToThemeMapping[volume.overrideTheme];
                                    break;
                                }
                            }
                        }
                    }

                    if (themeToUse != null)
                    {
                        PropBySocketType_t PropBySocketType = PropBySocketTypeByTheme[themeToUse];
                        if (PropBySocketType.ContainsKey(socket.SocketType))
                        {
                            var data = new NodeListExecutionData();
                            data.socket = socket;
                            data.nodeDataList = PropBySocketType[socket.SocketType];

                            if (ShouldDelayExecution(data.nodeDataList))
                            {
                                delayedExecutionList.Enqueue(data);
                            }
                            else
                            {
                                ExecuteNodesUnderMarker(data, nodesExecutionContext, spawnDataList);
                            }
                        }
                    }
                }

                // We execute the delayed node list (that have spatial constraints) at the very end of the list
                // Each execution of the delayed node however, can add more items to this list
                bool isLastIndex = (i == PropSockets.Count - 1);
                while (isLastIndex && delayedExecutionList.Count > 0) {
                    var data = delayedExecutionList.Dequeue();
                    if (!data.socket.markForDeletion)
                    {
                        ExecuteNodesUnderMarker(data, nodesExecutionContext, spawnDataList);
                    }

                    isLastIndex = (i == PropSockets.Count - 1);
                }
            }

            RecursivelyTagMarkersForDeletion();

            // Spawn the items
            foreach (var spawnData in spawnDataList)
            {
                if (spawnData.socket.markForDeletion)
                {
                    continue;
                }

                SpawnNodeItem(spawnData, nodesExecutionContext);
            }
        }

        void RecursivelyTagMarkerForDeletion(PropSocket marker, HashSet<int> visited)
        {
            visited.Add(marker.Id);
            marker.markForDeletion = true;
            foreach (var childMarker in marker.childMarkers)
            {
                if (!visited.Contains(childMarker.Id))
                {
                    RecursivelyTagMarkerForDeletion(childMarker, visited);
                }
            }
        }


        void RecursivelyTagMarkersForDeletion()
        {
            var visited = new HashSet<int>();
            foreach (var marker in PropSockets)
            {
                if (marker.markForDeletion && !visited.Contains(marker.Id))
                {
                    RecursivelyTagMarkerForDeletion(marker, visited);
                }
            }
        }

        bool ShouldDelayExecution(List<PropTypeData> nodeDataList)
        {
            // If we use a spatial constraint, delay the execution
            foreach (PropTypeData nodeData in nodeDataList)
            {
                if (nodeData.useSpatialConstraint && nodeData.spatialConstraint != null)
                {
                    return true;
                }
            }
            return false;
        }

        void SpawnNodeItem(NodeSpawnData data, NodeListExecutionContext context)
        {
            GameObject dungeonItem = null;
            var nodeData = data.nodeData;

            if (nodeData is GameObjectPropTypeData)
            {
                var gameObjectProp = nodeData as GameObjectPropTypeData;
                dungeonItem = context.SceneProvider.AddGameObject(gameObjectProp, data.transform, context.objectInstantiator);
            }
            else if (nodeData is GameObjectArrayPropTypeData)
            {
                var gameObjectArrayProp = nodeData as GameObjectArrayPropTypeData;
                int count = gameObjectArrayProp.Templates.Length;
                int index = Mathf.FloorToInt(random.GetNextUniformFloat() * count) % count;
                dungeonItem = context.SceneProvider.AddGameObjectFromArray(gameObjectArrayProp, index, data.transform, context.objectInstantiator);
            }
            else if (nodeData is SpritePropTypeData)
            {
                var spriteProp = nodeData as SpritePropTypeData;
                dungeonItem = context.SceneProvider.AddSprite(spriteProp, data.transform, context.objectInstantiator);
            }

            TagDungeonItemUserData(dungeonItem, data.socket.cellId);
        }

        void ExecuteNodesUnderMarker(NodeListExecutionData data, NodeListExecutionContext context, List<NodeSpawnData> spawnDataList)
        {
            var socket = data.socket;
            var nodeDataList = data.nodeDataList;
            foreach (PropTypeData nodeData in nodeDataList)
            {
                bool insertMesh = false;
                Matrix4x4 transform = socket.Transform * nodeData.Offset;

                if (nodeData.UseSelectionRule && nodeData.SelectorRuleClassName != null)
                {
                    var selectorRule = context.instanceCache.GetInstance(nodeData.SelectorRuleClassName) as SelectorRule;
                    if (selectorRule != null)
                    {
                        // Run the selection rule logic to determine if we need to insert this mesh in the scene
                        insertMesh = selectorRule.CanSelect(socket, transform, model, random.UniformRandom);
                    }
                }
                else
                {
                    // Perform probability based selection logic
                    float probability = context.srandom.GetNextUniformFloat();
                    insertMesh = (probability < nodeData.Affinity);
                }

                if (insertMesh && nodeData.useSpatialConstraint && nodeData.spatialConstraint != null)
                {
                    Matrix4x4 spatialOffset;
                    PropSocket[] markersToRemove;
                    if (!ProcessSpatialConstraint(context.constraintProcessor, nodeData.spatialConstraint, socket, out spatialOffset, out markersToRemove))
                    {
                        // Fails spatial constraint
                        insertMesh = false;
                    }
                    else
                    {
                        // Apply the offset
                        var markerOffset = socket.Transform;
                        if (nodeData.spatialConstraint != null && !nodeData.spatialConstraint.applyMarkerRotation)
                        {
                            var markerPosition = Matrix.GetTranslation(ref markerOffset);
                            var markerScale = Matrix.GetScale(ref markerOffset);
                            markerOffset = Matrix4x4.TRS(markerPosition, Quaternion.identity, markerScale);
                        }
                        transform = markerOffset * spatialOffset * nodeData.Offset;

                        foreach (var markerToRemove in markersToRemove)
                        {
                            markerToRemove.markForDeletion = true;
                        }
                    }
                }

                if (insertMesh)
                {
                    // Attach this prop to the socket
                    // Apply transformation logic, if specified
                    if (nodeData.UseTransformRule && nodeData.TransformRuleClassName != null && nodeData.TransformRuleClassName.Length > 0)
                    {
                        var transformer = context.instanceCache.GetInstance(nodeData.TransformRuleClassName) as TransformationRule;
                        if (transformer != null)
                        {
                            Vector3 _position, _scale;
                            Quaternion _rotation;
                            transformer.GetTransform(socket, model, transform, random.UniformRandom, out _position, out _rotation, out _scale);
                            var offset = Matrix4x4.TRS(_position, _rotation, _scale);
                            transform = transform * offset;
                        }
                    }

                    // Create a spawn request
                    var spawnData = new NodeSpawnData();
                    spawnData.nodeData = nodeData;
                    spawnData.transform = transform;
                    spawnData.socket = socket;
                    spawnDataList.Add(spawnData);

                    // Add child sockets if any
                    foreach (PropChildSocketData ChildSocket in nodeData.ChildSockets)
                    {
                        Matrix4x4 childTransform = transform * ChildSocket.Offset;
                        var childMarker = EmitMarker(ChildSocket.SocketType, childTransform, socket.gridPosition, socket.cellId);
                        data.socket.childMarkers.Add(childMarker);
                    }

                    if (nodeData.ConsumeOnAttach)
                    {
                        // Attach no more on this socket
                        break;
                    }
                }
            }
        }
    }
}
