# mix


mix performs a linear interpolation between

x and y using

a to weight between them. The return

value is computed as x \times (1 - a) + y \times a.

The variants of mix where

a is <code class="constant">genBType select

which vector each returned component comes from. For a component

of a that is false, the corresponding

component of x is returned. For a

component of a that is true, the

corresponding component of y is returned.

Components of x and

y that are not selected are allowed to be

invalid floating-point values and will have no effect on the

results.

## mix(float,float,float)

### Parameters

x
  Type: float
y
  Type: float
a
  Type: float

### Return Value

  Type: float

<details><summary>JSON</summary>

```
{
  "Type": "mix(float,float,float)",
  "Name": "mix(float,float,float)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "y",
      "Type": "float"
    },
    {
      "Connection": null,
      "Id": "a",
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

## mix(vec2,vec2,vec2)

### Parameters

x
  Type: vec2
y
  Type: vec2
a
  Type: vec2

### Return Value

  Type: vec2

<details><summary>JSON</summary>

```
{
  "Type": "mix(vec2,vec2,vec2)",
  "Name": "mix(vec2,vec2,vec2)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "y",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "a",
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

## mix(vec3,vec3,vec3)

### Parameters

x
  Type: vec3
y
  Type: vec3
a
  Type: vec3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "mix(vec3,vec3,vec3)",
  "Name": "mix(vec3,vec3,vec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "y",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "a",
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

## mix(vec4,vec4,vec4)

### Parameters

x
  Type: vec4
y
  Type: vec4
a
  Type: vec4

### Return Value

  Type: vec4

<details><summary>JSON</summary>

```
{
  "Type": "mix(vec4,vec4,vec4)",
  "Name": "mix(vec4,vec4,vec4)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "y",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "a",
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

## mix(vec2,vec2,float)

### Parameters

x
  Type: vec2
y
  Type: vec2
a
  Type: float

### Return Value

  Type: vec2

<details><summary>JSON</summary>

```
{
  "Type": "mix(vec2,vec2,float)",
  "Name": "mix(vec2,vec2,float)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "y",
      "Type": "vec2"
    },
    {
      "Connection": null,
      "Id": "a",
      "Type": "float"
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

## mix(vec3,vec3,float)

### Parameters

x
  Type: vec3
y
  Type: vec3
a
  Type: float

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "mix(vec3,vec3,float)",
  "Name": "mix(vec3,vec3,float)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "y",
      "Type": "vec3"
    },
    {
      "Connection": null,
      "Id": "a",
      "Type": "float"
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

## mix(vec4,vec4,float)

### Parameters

x
  Type: vec4
y
  Type: vec4
a
  Type: float

### Return Value

  Type: vec4

<details><summary>JSON</summary>

```
{
  "Type": "mix(vec4,vec4,float)",
  "Name": "mix(vec4,vec4,float)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "y",
      "Type": "vec4"
    },
    {
      "Connection": null,
      "Id": "a",
      "Type": "float"
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

