import * as React from 'react';
import classes from './Layout.module.scss';
import classnames from 'classnames';

type Props = React.PropsWithChildren<any>;

const Layout = (props: Props): React.ReactElement => {
  const { children, className } = props;

  return (
    <div className={classes.root}>
      <div className={classnames(classes.innerContainer, className)}>
        {children ? children : null}
      </div>
    </div>
  );
};

export default Layout;
