using UnityEngine;
using System.Collections;

namespace MonobitEngine.Sample
{
    public class Item : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            gameObject.transform.Rotate(0, 10.0f * Time.deltaTime, 0);
        }

        // プレハブがインスタンス化されたときのコールバック
        public void OnMonobitInstantiate(MonobitMessageInfo info)
        {
            ClientSide.RakeupGame.itemObject.Add(gameObject);
        }
    }
}
