# cross


cross returns the cross product of two

vectors, x and y,

i.e.

\begin{pmatrix}

{ x[1] \times y[2] - y[1] \times x[2] } \\

{ x[2] \times y[0] - y[2] \times x[0] } \\

{ x[0] \times y[1] - y[0] \times x[1] }

\end{pmatrix}.

## cross(vec3,vec3)

### Parameters

x
  Type: vec3
y
  Type: vec3

### Return Value

  Type: vec3

<details><summary>JSON</summary>

```
{
  "Type": "cross(vec3,vec3)",
  "Name": "cross(vec3,vec3)",
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

