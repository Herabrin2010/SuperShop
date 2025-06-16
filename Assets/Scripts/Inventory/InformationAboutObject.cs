using UnityEngine;

public class InformationAboutObject : MonoBehaviour
{
    public string _name;
    public Sprite _sprite;

    // Добавляем уникальный идентификатор
    public int ItemId => GetInstanceID();
}