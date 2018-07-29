# faceforward


faceforward orients a vector to point away from a surface as defined by its normal.

If <a class="citerefentry" href="dot.xhtml"><span class="citerefentry"><span class="refentrytitle">dot</span></span></a><code class="code">(Nref, I) < 0

faceforward returns N, otherwise it returns <code class="code">-N.

## faceforward(float,float,float)

### Parameters

N
  Type: float
I
  Type: float
Nref
  Type: float

### Return Value

  Type: float

<details><summary>JSON</summary>

```
{
  "Type": "faceforward(float,float,float)",
  "Name": "faceforward(float,float,float)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "N",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "I",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "Nref",
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

## faceforward(vec2,vec2,vec2)

### Parameters

N
  Type: vec2
I
  Type: vec2
Nref
  Type: vec2

### Return Value

  Type: vec2

<details><summary>JSON</summary>

```
{
  "Type": "faceforward(vec2,vec2,vec2)",
  "Name": "faceforward(vec2,vec2,vec2)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "N",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "I",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "Nref",
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

## faceforward(vec3,vec3,vec3)

### Parameters

N
  Type: vec3
I
  Type: vec3
Nref
  Type: vec3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "faceforward(vec3,vec3,vec3)",
  "Name": "faceforward(vec3,vec3,vec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "N",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "I",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "Nref",
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

## faceforward(vec4,vec4,vec4)

### Parameters

N
  Type: vec4
I
  Type: vec4
Nref
  Type: vec4

### Return Value

  Type: vec4

<details><summary>JSON</summary>

```
{
  "Type": "faceforward(vec4,vec4,vec4)",
  "Name": "faceforward(vec4,vec4,vec4)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "N",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "I",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "Nref",
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

