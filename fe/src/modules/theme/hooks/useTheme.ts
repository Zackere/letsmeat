import { GlobalState } from '../../../store';
import { Theme } from '../types';
import { useSelector } from 'react-redux';

export default function useTheme(): Theme | '' {
  return useSelector((state: GlobalState) => state.theme);
}
