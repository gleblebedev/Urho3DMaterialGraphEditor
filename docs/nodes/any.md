# any


any returns true if any element of x is true and false otherwise.

It is functionally equivalent to:

```
    bool any(bvec x) {     // bvec can be bvec2, bvec3 or bvec4
        bool result = false;
        int i;
        for (i = 0; i < x.length(); ++i) {
            result |= x[i];
        }
        return result;
    }
```

## any(bvec2)

### Parameter

x
  Type: bvec2

### Return Value

  Type: bool

<details><summary>JSON</summary>

```
{
  "Type": "any(bvec2)",
  "Name": "any(bvec2)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "bvec2"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "bool"
    }
  ]
}
```

</details>

## any(bvec3)

### Parameter

x
  Type: bvec3

### Return Value

  Type: bool

<details><summary>JSON</summary>

```
{
  "Type": "any(bvec3)",
  "Name": "any(bvec3)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "bvec3"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "bool"
    }
  ]
}
```

</details>

## any(bvec4)

### Parameter

x
  Type: bvec4

### Return Value

  Type: bool

<details><summary>JSON</summary>

```
{
  "Type": "any(bvec4)",
  "Name": "any(bvec4)",
  "Category": 1,
  "InputPins": [
    {
      "Connection": null,
      "Id": "x",
      "Type": "bvec4"
    }
  ],
  "OutputPins": [
    {
      "Id": "",
      "Type": "bool"
    }
  ]
}
```

</details>

