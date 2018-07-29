# reflect


For a given incident vector I and surface normal N reflect returns

the reflection direction calculated as <code class="code">I - 2.0 * dot(N, I) * N.

N should be normalized in order to achieve the desired result.

## reflect(float,float)

### Parameters

I
  Type: float
N
  Type: float

### Return Value

  Type: float

<details><summary>JSON</summary>

```
{
  "Type": "reflect(float,float)",
  "Name": "reflect(float,float)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "I",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "N",
      "Type": "float"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "float"
    }
  ]
}
```

</details>

## reflect(vec2,vec2)

### Parameters

I
  Type: vec2
N
  Type: vec2

### Return Value

  Type: vec2

<details><summary>JSON</summary>

```
{
  "Type": "reflect(vec2,vec2)",
  "Name": "reflect(vec2,vec2)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "I",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "N",
      "Type": "vec2"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "vec2"
    }
  ]
}
```

</details>

## reflect(vec3,vec3)

### Parameters

I
  Type: vec3
N
  Type: vec3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "reflect(vec3,vec3)",
  "Name": "reflect(vec3,vec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "I",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "N",
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

## reflect(vec4,vec4)

### Parameters

I
  Type: vec4
N
  Type: vec4

### Return Value

  Type: vec4

<details><summary>JSON</summary>

```
{
  "Type": "reflect(vec4,vec4)",
  "Name": "reflect(vec4,vec4)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "I",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "N",
      "Type": "vec4"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "vec4"
    }
  ]
}
```

</details>

