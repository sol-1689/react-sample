import React from 'react'
import { Select, Input, MenuItem, Checkbox, ListItemText, TextField } from '@mui/material';

//デフォルトのセレクトリストを作成する。valueTextListは{Value, Text}のオブジェクトの想定。
export const DefaultSelectList = ({ valueTextList, showNoSelect, ...otherProps }) => {

  if (otherProps.hasOwnProperty('value'))
    otherProps = { ...otherProps, value: otherProps.value == null ? "" : otherProps.value }

  return (
    <TextField select {...otherProps}>
      {showNoSelect && <MenuItem value=""><em>--未選択--</em></MenuItem>}
      {
        valueTextList.map(each => (
          <MenuItem key={each.Value} value={each.Value} disabled={each.DisplayDisabled}>{each.Text}</MenuItem>
        ))
      }
    </TextField>
  )
}

export const DefaultGroupSelectList = ({
    gruopItemList,
    showNoSelect,
    ...otherProps
  }) => {
    return (
      //valueが数値の場合に備え文字列に変換しておく。
      <Select native  {...{...otherProps, value: "" + otherProps.value}}>
        {showNoSelect && <option key="" value="">--未選択--</option>}
        {gruopItemList.map(eachG => (
          <optgroup key={eachG.GroupName} label={eachG.GroupName}>
            {eachG.Items.map(each => (
              <option key={each.Value} value={each.Value} disabled={each.DisplayDisabled}> 
                {each.Text}
              </option>
            ))}
          </optgroup>
        ))}
      </Select>
    );
  };


//複数選択セレクトリストを作成する。
export const DefaultMultiSelectList = ({valueTextList, value, showNoSelect, ...otherProps}) => {
    const adjustedValue = value.map(v => v + ""); //数値の場合に文字列に変換しておく。(これしないと初期値のチェックが外れないケースがある)

    return (
        <Select
        multiple
        input={<Input />}
        renderValue={selected => selected.map(v => valueTextList.find(vt => vt.Value == v)?.Text).join(', ')}
        //MenuProps={MenuProps}
        value={adjustedValue}
        {...otherProps}
        >
            {valueTextList.map(each => (
                <MenuItem key={each.Value} value={each.Value} disabled={each.DisplayDisabled}>
                    <Checkbox checked={!!adjustedValue.find(v => v == each.Value)} />
                    <ListItemText primary={each.Text} />
                </MenuItem>
            ))}
        </Select>
    )
}

const ITEM_HEIGHT = 48;
const ITEM_PADDING_TOP = 8;
const MenuProps = {
  PaperProps: {
    style: {
    //   maxHeight: ITEM_HEIGHT * 4.5 + ITEM_PADDING_TOP,
      width: 250,
    },
  },
};
