# refract


For a given incident vector I, surface normal N and ratio of

indices of refraction, eta, refract returns the refraction vector,

R.

R is calculated as:

```
    k = 1.0 - eta * eta * (1.0 - dot(N, I) * dot(N, I));
    if (k < 0.0)
        R = genType(0.0);       // or genDType(0.0)
    else
        R = eta * I - (eta * dot(N, I) + sqrt(k)) * N;
```

The input parameters I and N should be normalized in order to achieve the desired result.

## refract(float,float,float)

### Parameters

I
  Type: float
N
  Type: float
eta
  Type: float

### Return Value

  Type: float

<details><summary>JSON</summary>

```
{
  "Type": "refract(float,float,float)",
  "Name": "refract(float,float,float)",
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
    },
    {
      "Connection": null,
      "Id": "eta",
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

## refract(vec2,vec2,float)

### Parameters

I
  Type: vec2
N
  Type: vec2
eta
  Type: float

### Return Value

  Type: vec2

<details><summary>JSON</summary>

```
{
  "Type": "refract(vec2,vec2,float)",
  "Name": "refract(vec2,vec2,float)",
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
    },
    {
      "Connection": null,
      "Id": "eta",
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

## refract(vec3,vec3,float)

### Parameters

I
  Type: vec3
N
  Type: vec3
eta
  Type: float

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "refract(vec3,vec3,float)",
  "Name": "refract(vec3,vec3,float)",
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
    },
    {
      "Connection": null,
      "Id": "eta",
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

## refract(vec4,vec4,float)

### Parameters

I
  Type: vec4
N
  Type: vec4
eta
  Type: float

### Return Value

  Type: vec4

<details><summary>JSON</summary>

```
{
  "Type": "refract(vec4,vec4,float)",
  "Name": "refract(vec4,vec4,float)",
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
    },
    {
      "Connection": null,
      "Id": "eta",
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

