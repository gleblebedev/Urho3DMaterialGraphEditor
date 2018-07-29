# smoothstep


smoothstep performs smooth Hermite interpolation between 0 and

1 when edge0 < x < edge1.

This is useful in cases where a threshold function with a smooth transition is desired.

smoothstep is equivalent to:

```
    genType t;  /* Or genDType t; */
    t = clamp((x - edge0) / (edge1 - edge0), 0.0, 1.0);
    return t * t * (3.0 - 2.0 * t);
```

Results are undefined if edge0 â‰¥ edge1.

## smoothstep(float,float,float)

### Parameters

edge0
  Type: float
edge1
  Type: float
x
  Type: float

### Return Value

  Type: float

<details><summary>JSON</summary>

```
{
  "Type": "smoothstep(float,float,float)",
  "Name": "smoothstep(float,float,float)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "edge0",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "edge1",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "x",
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

## smoothstep(vec2,vec2,vec2)

### Parameters

edge0
  Type: vec2
edge1
  Type: vec2
x
  Type: vec2

### Return Value

  Type: vec2

<details><summary>JSON</summary>

```
{
  "Type": "smoothstep(vec2,vec2,vec2)",
  "Name": "smoothstep(vec2,vec2,vec2)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "edge0",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "edge1",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "x",
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

## smoothstep(vec3,vec3,vec3)

### Parameters

edge0
  Type: vec3
edge1
  Type: vec3
x
  Type: vec3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "smoothstep(vec3,vec3,vec3)",
  "Name": "smoothstep(vec3,vec3,vec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "edge0",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "edge1",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "x",
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

## smoothstep(vec4,vec4,vec4)

### Parameters

edge0
  Type: vec4
edge1
  Type: vec4
x
  Type: vec4

### Return Value

  Type: vec4

<details><summary>JSON</summary>

```
{
  "Type": "smoothstep(vec4,vec4,vec4)",
  "Name": "smoothstep(vec4,vec4,vec4)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "edge0",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "edge1",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "x",
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

## smoothstep(float,float,vec2)

### Parameters

edge0
  Type: float
edge1
  Type: float
x
  Type: vec2

### Return Value

  Type: vec2

<details><summary>JSON</summary>

```
{
  "Type": "smoothstep(float,float,vec2)",
  "Name": "smoothstep(float,float,vec2)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "edge0",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "edge1",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "x",
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

## smoothstep(float,float,vec3)

### Parameters

edge0
  Type: float
edge1
  Type: float
x
  Type: vec3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "smoothstep(float,float,vec3)",
  "Name": "smoothstep(float,float,vec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "edge0",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "edge1",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "x",
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

## smoothstep(float,float,vec4)

### Parameters

edge0
  Type: float
edge1
  Type: float
x
  Type: vec4

### Return Value

  Type: vec4

<details><summary>JSON</summary>

```
{
  "Type": "smoothstep(float,float,vec4)",
  "Name": "smoothstep(float,float,vec4)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "edge0",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "edge1",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "x",
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

