import { createTheme } from "@mui/material";
import { deepOrange } from "@mui/material/colors";

const existingTheme = createTheme({
palette: {
    divider: '#212121'
},
typography: {
    h5: {
    color: 'black'
    },
    h6: {
    color: 'black'
    }
}
});

const darkModeTheme = createTheme({
palette: {
    mode: 'dark',
    divider: deepOrange[700]
},
typography: {
    h5: {
    color: 'white'
    },
    h6: {
    color: 'white'
    },
}
});

export { darkModeTheme, existingTheme };