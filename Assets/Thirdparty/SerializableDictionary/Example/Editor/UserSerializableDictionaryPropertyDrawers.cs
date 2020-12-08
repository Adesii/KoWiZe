using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static citySystem;
using static SFXManagerController;

[CustomPropertyDrawer(typeof(StringSFXDictionary))]
[CustomPropertyDrawer(typeof(ResResClassDictionary))]
[CustomPropertyDrawer(typeof(StringStringDictionary))]
[CustomPropertyDrawer(typeof(ObjectColorDictionary))]
[CustomPropertyDrawer(typeof(StringColorArrayDictionary))]
public class AnySerializableDictionaryPropertyDrawer : SerializableDictionaryPropertyDrawer {}

[CustomPropertyDrawer(typeof(ColorArrayStorage))]
public class AnySerializableDictionaryStoragePropertyDrawer: SerializableDictionaryStoragePropertyDrawer {}
