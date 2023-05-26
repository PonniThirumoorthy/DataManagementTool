import React from 'react';
import { css } from '@emotion/react';
import { BeatLoader } from 'react-spinners';

function Loading(props) {
    const overlayStyles = css`
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background-color: rgba(0, 0, 0, 0.5);
    display: flex;
    justify-content: center;
    align-items: center;
    z-index: 9999;
  `;

    const spinnerStyles = css`
    margin-right: 10px;
  `;

    return (
        <div css={overlayStyles}>
            <BeatLoader css={spinnerStyles} color="#1976D2" loading={props.loading} size={30} />
        </div>
    );
}

export default Loading;


