using UnityEngine;
using UnityEngine.Assertions;
using System.Collections.Generic;

namespace Bright
{
	/// <summary>
	/// チャンクコンポーネント.
	/// </summary>
	[ExecuteInEditMode][RequireComponent(typeof(ChunkCreator), typeof(OnTriggerEnter2DHiddenRelatedChunk))]
	public class Chunk : ChunkBase
	{
		[SerializeField]
		private ChunkDoorway doorway;

		private List<BlankChunk> blankChunks = new List<BlankChunk>();

#if UNITY_EDITOR

		private StageManager _stageManager;

		private ChunkCreator _creator;

		private ChunkDoorway _doorway = new ChunkDoorway();

		private GameObject _left;
		private GameObject _right;
		private GameObject _top;
		private GameObject _bottom;

		void Update()
		{
			if(Application.isPlaying)
			{
				return;
			}

			if(this._stageManager == null)
			{
				this._stageManager = GameObject.Find("Stage").GetComponent<StageManager>();
			}
			if(this._creator == null)
			{
				this._creator = GetComponent<ChunkCreator>();
			}

			if(!this._creator.ReadyEdit)
			{
				Debug.LogWarning("ChunkCreatorの準備が出来ていないためエディター機能をオフにします.");
				return;
			}

			var maxX = StageManager.ChunkSizeX - 1;
			var maxY = StageManager.ChunkSizeY - 1;
			this._Create(GameDefine.DirectionType.Left, ref this._left, () =>
			{
				return CreateWall(this._creator, this._stageManager, GameDefine.DirectionType.Left, Point.Zero, Point.Zero);
			});
			this._Create(GameDefine.DirectionType.Right, ref this._right, () =>
			{
				return CreateWall(this._creator, this._stageManager, GameDefine.DirectionType.Right, Point.Zero, Point.Right * maxX);
			});
			this._Create(GameDefine.DirectionType.Top, ref this._top, () =>
			{
				return CreateGround(this._creator, this._stageManager, GameDefine.DirectionType.Top, Point.Zero, Point.Top * maxY);
			});
			this._Create(GameDefine.DirectionType.Bottom, ref this._bottom, () =>
			{
				return CreateGround(this._creator, this._stageManager, GameDefine.DirectionType.Bottom, Point.Zero, Point.Zero);
			});

			this._doorway.Copy(this.doorway);
		}

		void OnDestroy()
		{
			if(Application.isPlaying)
			{
				return;
			}

			DestroyImmediate(this._left);
			DestroyImmediate(this._right);
			DestroyImmediate(this._top);
			DestroyImmediate(this._bottom);
		}

		void _Create(GameDefine.DirectionType type, ref GameObject go, System.Func<GameObject> createFunction)
		{
			if(!this.doorway.Difference(this._doorway, type))
			{
				return;
			}

			DestroyImmediate(go);

			if(this.doorway.CanCreate(type))
			{
				go = createFunction();
				go.transform.parent = null;
				go.hideFlags = HideFlags.HideInHierarchy;
			}
		}

		[ContextMenu("Setup")]
		void Setup()
		{
			this.gameObject.layer = LayerMask.NameToLayer("Chunk");

			var rigidBody2D = Attach<Rigidbody2D>();
			rigidBody2D.isKinematic = true;

			var collider = Attach<BoxCollider2D>();
			collider.size = new Vector2(StageManager.ChunkSizeX, StageManager.ChunkSizeY);
			collider.offset = new Vector2(StageManager.ChunkSizeX / 2, StageManager.ChunkSizeY / 2);
			collider.isTrigger = true;

			Attach<ChunkCreator>();
			Attach<OnTriggerEnter2DVisibleChunk>();
			Attach<OnTriggerEnter2DHiddenRelatedChunk>();
			Attach<OnTriggerEnter2DCreateNextChunk>();
		}

		T Attach<T>() where T : Component
		{
			var t = GetComponent<T>();
			if(t == null)
			{
				t = gameObject.AddComponent<T>();
			}

			return t;
		}

#endif

		public void Initialize(StageManager stageManager, Point index, BlankChunk blankChunk)
		{
			if(blankChunk != null)
			{
				this.node = blankChunk.Node;
			}

			this.stageManager = stageManager;
			this.index = index;

			var creator = GetComponent<ChunkCreator>();
			var maxX = StageManager.ChunkSizeX - 1;
			var maxY = StageManager.ChunkSizeY - 1;

			this.transform.position = stageManager.GetPosition(index, Point.Zero);

			this.CreateWall(creator, stageManager, GameDefine.DirectionType.Left, index, Point.Zero);
			this.CreateWall(creator, stageManager, GameDefine.DirectionType.Right, index, Point.Right * maxX);
			this.CreateGround(creator, stageManager, GameDefine.DirectionType.Top, index, Point.Top * maxY);
			this.CreateGround(creator, stageManager, GameDefine.DirectionType.Bottom, index, Point.Zero);
			this.CreateBlankChunk(stageManager, GameDefine.DirectionType.Left, index + Point.Left);
			this.CreateBlankChunk(stageManager, GameDefine.DirectionType.Right, index + Point.Right);
			this.CreateBlankChunk(stageManager, GameDefine.DirectionType.Top, index + Point.Top);
			this.CreateBlankChunk(stageManager, GameDefine.DirectionType.Bottom, index + Point.Bottom);
			
			var components = GetComponentsInChildren(typeof(IReceiveOnInitializeChunk));
			foreach(var component in components)
			{
				(component as IReceiveOnInitializeChunk).OnInitializeChunk(stageManager, index);
			}
		}

		public void Visible(Chunk ignoreChunk)
		{
			this.node.Visible(ignoreChunk);
		}

		public void Hidden(Chunk ignoreChunk)
		{
			this.node.Hidden(ignoreChunk);
		}

		public void VisibleRelatedChunk()
		{
			this.node.VisibleRelatedChunk(this);
		}

		public void HiddenRelatedChunk()
		{
			this.node.HiddenRelatedChunk(this);
		}

		public bool IsOpen(ChunkDoorway doorway)
		{
			return this.doorway.IsOpen(doorway);
		}

		public void Hypostatization()
		{
			this.blankChunks.ForEach(b => b.Hypostatization());
		}

		public void DestroyBlankChunk()
		{
			this.blankChunks.ForEach(b => Destroy(b.gameObject));
		}

		private GameObject CreateGround(ChunkCreator creator, StageManager stageManager, GameDefine.DirectionType direction, Point chunkIndex, Point position)
		{
			if(!this.doorway.CanCreate(direction))
			{
				return null;
			}

			return creator.CreateGround(stageManager, chunkIndex, position);
		}

		private GameObject CreateWall(ChunkCreator creator, StageManager stageManager, GameDefine.DirectionType direction, Point chunkIndex, Point position)
		{
			if(!this.doorway.CanCreate(direction))
			{
				return null;
			}

			return creator.CreateWall(stageManager, chunkIndex, position);
		}

		private void CreateBlankChunk(StageManager stageManager, GameDefine.DirectionType direction, Point chunkIndex)
		{
			if(this.doorway.CanCreate(direction))
			{
				return;
			}

			if(this.node.Contains(direction))
			{
				return;
			}

			this.blankChunks.Add(stageManager.CreateBlankChunk(this, GameDefine.InverseDirection(direction), chunkIndex));
		}
	}
}
