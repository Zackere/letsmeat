import * as React from 'react';
import { detectUserPreference } from '../state/actions';
import { useDispatch } from 'react-redux';
import classes from './ThemedContainer.module.scss';
import useTheme from '../hooks/useTheme';

type Props = React.PropsWithChildren<any>;

const ThemedContainer = (props: Props): React.ReactElement => {
  const { children } = props;
  const dispatch = useDispatch();
  const theme = useTheme();
  const className = theme === 'dark' ? classes.darkTheme : classes.lightTheme;

  React.useEffect(() => {
    dispatch(detectUserPreference());
  }, [dispatch]);

  React.useEffect(() => {
    const root = document.querySelector('#root') as HTMLDivElement;
    root.classList.remove(classes.darkTheme);
    root.classList.remove(classes.lightTheme);
    root.classList.add(className);
  }, [className]);

  return children;
};

export default ThemedContainer;
