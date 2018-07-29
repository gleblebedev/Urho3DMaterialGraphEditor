# getWorldNormal

## getWorldNormal


### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "getWorldNormal",
  "Name": "getWorldNormal",
  "Category": 1,
  "InputPins": [],
  "OutputPins": [
    {
      "Id": "",
      "Type": "vec3"
    }
  ]
}
```

</details>

## getWorldNormal(vec3,vec4,vec3)

### Parameters

decodedNormal
  Type: vec3
vertexTangent
  Type: vec4
vertexNormal
  Type: vec3
normalMatrix
  Type: mat3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "getWorldNormal(vec3,vec4,vec3)",
  "Name": "getWorldNormal(vec3,vec4,vec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "decodedNormal",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "vertexTangent",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "vertexNormal",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "normalMatrix",
      "Type": "mat3"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "vec3"
    }
  ]
}
```

</details>

## getWorldNormal(vec3)

### Parameter

vertexNormal
  Type: vec3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "getWorldNormal(vec3)",
  "Name": "getWorldNormal(vec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "vertexNormal",
      "Type": "vec3"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "vec3"
    }
  ]
}
```

</details>

