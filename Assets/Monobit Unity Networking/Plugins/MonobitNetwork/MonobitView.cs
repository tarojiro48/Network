using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using MonobitEngine.Extensions;

namespace MonobitEngine
{
#if UNITY_5_0
	using Debug = DebugExt;
#endif

	/// <summary>
	/// MonobitViewクラス
	/// </summary>
	[AddComponentMenu("Monobit Networking/Monobit View &v")]
	public class MonobitView : MonobitEngineBase.MonobitView
	{
        /// <summary>
        /// ストリームデータをシリアライズする
        /// </summary>
        /// <param name="stream">ストリームデータ</param>
        /// <param name="info">メッセージ情報</param>
        public override void SerializeView(MonobitStream stream, MonobitMessageInfo info)
        {
            Serialize(observed, stream, info);
            foreach (Component component in InternalObservedComponents)
            {
                Serialize(component, stream, info);
            }
            foreach (Component component in ObservedComponents)
            {
                Serialize(component, stream, info);
            }
        }

		/// <summary>
		/// 指定コンポーネントごとにストリームデータをシリアライズする
		/// </summary>
		/// <param name="component">指定コンポーネント</param>
		/// <param name="stream">ストリームデータ</param>
		/// <param name="info">メッセージ情報</param>
		private void Serialize(Component component, MonobitStream stream, MonobitMessageInfo info)
        {
            if (component == null)
                return;

            new Switch(component)
                .Case<MonobitAnimatorView>(p =>
                {
                    ExecuteComponentOnSerialize<MonobitAnimatorView>(component, stream, info);
                })
                .Case<MonobitTransformView>(p =>
                {
                    ExecuteComponentOnSerialize<MonobitTransformView>(component, stream, info);
                })
                .Case<MonoBehaviour>(p =>
                {
                    ExecuteComponentOnSerialize<MonoBehaviour>(component, stream, info);
                })
                .Case<Transform>(p =>
                {
                    Serialize<Transform, MonobitEngineBase.OnSerializeTransform>(
                        p,
                        stream,
                        new Serializer<Transform>[] {
                            trans => { return trans.localPosition; },
                            trans => { return trans.localRotation; },
                            trans => { return trans.localScale; },
                        });
                })
                .Case<Rigidbody>(p =>
                {
                    Serialize<Rigidbody, MonobitEngineBase.OnSerializeRigidBody>(
                        p,
                        stream,
                        new Serializer<Rigidbody>[] {
                            rigid => { return rigid.velocity; },
                            rigid => { return rigid.angularVelocity; },
                        });
                })
                .Case<Rigidbody2D>(p =>
                {
                    Serialize<Rigidbody2D, MonobitEngineBase.OnSerializeRigidBody>(
                        p,
                        stream,
                        new Serializer<Rigidbody2D>[] {
                            rigid => { return rigid.velocity; },
                            rigid => { return rigid.angularVelocity; },
                        });
                });
        }

		/// <summary>
		/// シリアライズ用デリゲート
		/// </summary>
		/// <typeparam name="T">コンポーネントの型</typeparam>
		/// <param name="component">コンポーネント</param>
		/// <returns></returns>
		private delegate object Serializer<T>(T component);

		/// <summary>
		/// コンポーネントよりデータ取得してシリアライズする
		/// </summary>
		/// <typeparam name="T1">コンポーネントの型</typeparam>
		/// <typeparam name="T2">指定シリアライズタイプ</typeparam>
		/// <param name="component">コンポーネント</param>
		/// <param name="stream">ストリームデータ</param>
		/// <param name="procs">コンポーネントよりデータ取得するデリゲート</param>
		private void Serialize<T1, T2>(T1 component, MonobitStream stream, params Serializer<T1>[] procs)
        {
            if (stream == null || procs == null || procs.Length == 0)
                return;

            if (typeof(T2) == typeof(MonobitEngineBase.OnSerializeTransform))
            {
                foreach (Serializer<T1> proc in procs)
                {
                    stream.Enqueue(proc(component));
                }
            }
            else if (typeof(T2) == typeof(MonobitEngineBase.OnSerializeRigidBody))
            {
                foreach (Serializer<T1> proc in procs)
                {
                    stream.Enqueue(proc(component));
                }
            }
        }

		/// <summary>
		/// ストリームデータをデシリアライズする
		/// </summary>
		/// <param name="stream">ストリームデータ</param>
		/// <param name="info">メッセージ情報</param>
		public override void DeserializeView(MonobitStream stream, MonobitMessageInfo info)
        {
            Desirialize(observed, stream, info);
            foreach (Component component in InternalObservedComponents)
            {
                Desirialize(component, stream, info);
            }
            foreach (Component component in ObservedComponents)
            {
                Desirialize(component, stream, info);
            }
        }

		/// <summary>
		/// 指定コンポーネントごとにストリームデータをデシリアライズする
		/// </summary>
		/// <param name="component">コンポーネント</param>
		/// <param name="stream">ストリームデータ</param>
		/// <param name="info">メッセージ情報</param>
		private void Desirialize(Component component, MonobitStream stream, MonobitMessageInfo info)
        {
            if (component == null)
                return;

            new Switch(component)
                .Case<MonobitAnimatorView>(p =>
                {
                    ExecuteComponentOnSerialize<MonobitAnimatorView>(component, stream, info);
                })
                .Case<MonobitTransformView>(p =>
                {
                    ExecuteComponentOnSerialize<MonobitTransformView>(component, stream, info);
                })
                .Case<MonoBehaviour>(p =>
                {
                    ExecuteComponentOnSerialize<MonoBehaviour>(component, stream, info);
                })
                .Case<Transform>(p => {
                    Desirialize<MonobitEngineBase.OnSerializeTransform>(
                        stream,
                        new Desirializer[] {
                            r => {
								// Position
								p.localPosition = (Vector3)r;
                            },
                            r => {
								// Rotation
								p.localRotation = (Quaternion)r;
                            },
                            r => {
								// Scale
								p.localScale = (Vector3)r;
                            }
                        });
                })
                .Case<Rigidbody>(p => {
                    Desirialize<MonobitEngineBase.OnSerializeRigidBody>(
                        stream,
                        new Desirializer[] {
                            r => {
								// angularVelocity
								p.angularVelocity = (Vector3)r;
                            },
                            r => {
								// velocity
								p.velocity = (Vector3)r;
                            }
                        });
                })
                .Case<Rigidbody2D>(p => {
                    Desirialize<MonobitEngineBase.OnSerializeRigidBody>(
                        stream,
                        new Desirializer[] {
                            r => {
								// angularVelocity
								p.angularVelocity = (float)r;
                            },
                            r => {
								// velocity
								p.velocity = (Vector2)r;
                            }
                        });
                });
        }

		/// <summary>
		/// デシリアライズ用デリゲート
		/// </summary>
		/// <param name="receive">設定データ</param>
        private delegate void Desirializer(object receive);

		/// <summary>
		/// ストリームデータから指定形式にデシリアライズする
		/// </summary>
		/// <typeparam name="T">指定デシリアライズタイプ</typeparam>
		/// <param name="stream">ストリームデータ</param>
		/// <param name="procs">ストリームからのデータを設定するデリゲート</param>
		private void Desirialize<T>(MonobitStream stream, params Desirializer[] procs)
        {
            if (stream == null || procs == null || procs.Length == 0)
                return;
            if (typeof(T) == typeof(MonobitEngineBase.OnSerializeTransform))
            {
                foreach (Desirializer proc in procs)
                {
                    proc(stream.Dequeue());
                }
            }
            else if (typeof(T) == typeof(MonobitEngineBase.OnSerializeRigidBody))
            {
                for (int i = 0; i < 2; i++)
                {
                    procs[i](stream.Dequeue());
                }
            }
        }

		/// <summary>
		/// OnMonobitSerializeViewを呼び出してシリアライズさせる
		/// </summary>
		/// <typeparam name="T">呼び出し先のクラス型</typeparam>
		/// <param name="component">コンポーネント</param>
		/// <param name="stream">ストリームデータ</param>
		/// <param name="info">メッセージ情報</param>
		private void ExecuteComponentOnSerialize<T>(Component component, MonobitStream stream, MonobitMessageInfo info)
        {
            if (component == null)
                return;

            MethodInfo methodInfo = null;
            if (m_dicOnSerializeMethodInfos.TryGetValue(component, out methodInfo) != true)
            {
                var result = MonobitNetwork.GetMethod<T>(component, MonobitNetworkingMessage.OnMonobitSerializeView.ToString(), out methodInfo);
                if (result == false)
                {
                    methodInfo = null;
                }
                m_dicOnSerializeMethodInfos.Add(component, methodInfo);
            }

            if (methodInfo != null)
            {
                string componentName = component.GetType().Name;
                if (stream.isWriting)
                {
                    stream.Enqueue(new string[] { componentName, "start" });
                }
                else
                {
                    stream.SkipDequeue();
                }
                methodInfo.Invoke(component, new object[] { stream, info });
                if (stream.isWriting)
                {
                    stream.Enqueue(new string[] { componentName, "end" });
                }
                else
                {
                    stream.SkipDequeue();
                }
            }
        }
    }
}
