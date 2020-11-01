import * as React from 'react';
import classes from './Logo.module.scss';
import classnames from 'classnames';

type Props = {
  className?: string,
}

const Logo = (props: Props): React.ReactElement => {
  const { className } = props;

  return (
    <div className={classnames(classes.root, className)}>
      <svg xmlns={'http://www.w3.org/2000/svg'} viewBox={'0 0 1700 370'}>
        {/* eslint-disable-next-line max-len */}
        <path fill={'currentColor'} d={'M287 246c-34 56-107 74-163 41l-2-2 163-163 48-48a185 185 0 1 0 17 195l-63-23zM185 66c17 0 34 4 50 11L77 235A119 119 0 0 1 185 66z'}/>
        <circle fill={'currentColor'} cx={'461'} cy={'320'} r={'50'}/>
        <text transform={'translate(573 272)'} className={classes.text}>
          EXAMINE
        </text>
      </svg>
    </div>
  );
};

export default Logo;
